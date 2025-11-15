using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using URLShortener_Application.Interfaces.Repositories;
using URLShortener_Application.Interfaces.Services;
using URLShortener_Application.Interfaces.Services.Helpers;
using URLShortener_Domain.Entities;
using URLShortener_Shared.DTOs;

namespace URLShortener_Application.Services
{
    public class ShortUrlService: IShortUrlService
    {
        private readonly IShortUrlRepository _shortUrlRepository;

        public ShortUrlService(IShortUrlRepository shortUrlRepository)
        {
            _shortUrlRepository = shortUrlRepository;
        }
        public async Task<Dto_ShortUrl> CreateShortUrlAsync(Dto_CreateShortUrl dto)
        {
            // If custom alias is provided, check if it already exists
            string shortCode = string.Empty;
            if (!string.IsNullOrWhiteSpace(dto.CustomAlias))
            {
                var existing = await _shortUrlRepository.GetByShortCodeAsync(dto.CustomAlias);
                if (existing != null)
                {
                    throw new Exception("Custom alias is already in use.");
                }
                    

                shortCode = dto.CustomAlias;
            }
            else
            {
                // Generate short code from Id
                var shortUrl = new ShortUrl
                {
                    OriginalUrl = dto.OriginalUrl,
                    UserId = dto.UserId,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = dto.ExpiresAt,
                    IsActive = true
                };

                await _shortUrlRepository.AddAsync(shortUrl);
                await _shortUrlRepository.SaveChangesAsync();

                shortCode = ShortCodeGenerator.Encode(shortUrl.ShortUrlId);
                shortUrl.ShortCode = shortCode;
                _shortUrlRepository.Update(shortUrl);
                await _shortUrlRepository.SaveChangesAsync();
            }

            // Map to DTO
            var createdDto = new Dto_ShortUrl
            {
                OriginalUrl = dto.OriginalUrl,
                CustomAlias = dto.CustomAlias,
                ShortCode = shortCode,
                ExpiresAt = dto.ExpiresAt,
                UserId = dto.UserId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ClickCount = 0
            };

            return createdDto;
        }

        public async Task<bool> DeleteShortUrlAsync(long id, int currentUserId)
        {
            var entity = await _shortUrlRepository.GetByIdAsync(id);
            if (entity == null) return false;

            if (entity.UserId == null || entity.UserId != currentUserId)
            {
                //Intentionally throw UnauthorizedAccessException because API want to return 403 Forbidden
                throw new UnauthorizedAccessException("You are not allowed to delete this short URL.");
            }
                

            _shortUrlRepository.Delete(entity);
            await _shortUrlRepository.SaveChangesAsync();
            return true;
        }

        public async Task<Dto_ShortUrl?> GetByIdAsync(long id)
        {
            var entity = await _shortUrlRepository.GetByIdAsync(id);
            if (entity == null) return null;
            return new Dto_ShortUrl
            {
                ShortUrlId = entity.ShortUrlId,
                OriginalUrl = entity.OriginalUrl,
                ShortCode = entity.ShortCode,
                CustomAlias = entity.CustomAlias,
                CreatedAt = entity.CreatedAt,
                ExpiresAt = entity.ExpiresAt,
                IsActive = entity.IsActive,
                ClickCount = entity.ClickCount,
                UserId = entity.UserId
            };
        }

        public async Task<Dto_ShortUrl?> GetByShortCodeAsync(string shortCode)
        {
            var result = await _shortUrlRepository.GetByShortCodeAsync(shortCode);
            if (result == null) return null;

            return new Dto_ShortUrl
            {
                ShortUrlId = result.ShortUrlId,
                OriginalUrl = result.OriginalUrl,
                ShortCode = result.ShortCode,
                CustomAlias = result.CustomAlias,
                CreatedAt = result.CreatedAt,
                ExpiresAt = result.ExpiresAt,
                IsActive = result.IsActive,
                ClickCount = result.ClickCount,
                UserId = result.UserId
            };
        }

        public async Task<IEnumerable<Dto_ShortUrl>> GetByUserIdAsync(int userId)
        {
            var entities = await _shortUrlRepository.GetByUserIdAsync(userId);
            var dtos = new List<Dto_ShortUrl>();
            if (entities.Any())
            {
                dtos = entities.Select(entity => new Dto_ShortUrl
                {
                    ShortUrlId = entity.ShortUrlId,
                    OriginalUrl = entity.OriginalUrl,
                    ShortCode = entity.ShortCode,
                    CustomAlias = entity.CustomAlias,
                    CreatedAt = entity.CreatedAt,
                    ExpiresAt = entity.ExpiresAt,
                    IsActive = entity.IsActive,
                    ClickCount = entity.ClickCount,
                    UserId = entity.UserId
                }).ToList();
            }
            return dtos;
        }

        public async Task<bool> SetActiveStatusAsync(long id, bool isActive, int currentUserId)
        {
            var entity = await _shortUrlRepository.GetByIdAsync(id);
            if (entity == null) return false;

            if (entity.UserId == null || entity.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You are not allowed to modify this short URL.");
            }
                
            entity.IsActive = isActive;
            _shortUrlRepository.Update(entity);
            await _shortUrlRepository.SaveChangesAsync();
            return true;
        }

        public async Task<Dto_ShortUrl?> UpdateShortUrlAsync(Dto_UpdateShortUrl dto, int currentUserId)
        {
            
            var entity = await _shortUrlRepository.GetByIdAsync(dto.ShortUrlId);
            if (entity == null) return null;

            // Only owner can update. Guest links (UserId == null) cannot be edited.
            if (entity.UserId == null || entity.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You are not allowed to update this short URL.");
            }
               
            // Validate OriginalUrl
            if (string.IsNullOrWhiteSpace(dto.OriginalUrl) || !Uri.TryCreate(dto.OriginalUrl, UriKind.Absolute, out _))
            {
                throw new ArgumentException("Invalid original URL.");
            }
               

            // Validate custom alias if provided and changed
            if (!string.IsNullOrWhiteSpace(dto.CustomAlias) && !string.Equals(dto.CustomAlias, entity.CustomAlias, StringComparison.Ordinal))
            {
                if (!Regex.IsMatch(dto.CustomAlias, "^[a-zA-Z0-9_-]+$"))
                {
                    throw new ArgumentException("Custom alias can only contain letters, numbers, hyphens, and underscores.");
                }
                    
                //Check if alias already exists
                var aliasExists = await _shortUrlRepository.GetByShortCodeAsync(dto.CustomAlias);
                if (aliasExists != null && aliasExists.ShortUrlId != entity.ShortUrlId)
                {
                    throw new ArgumentException("Custom alias is already in use.");
                }
                    

                // Apply new custom alias
                entity.CustomAlias = dto.CustomAlias;
                entity.ShortCode = dto.CustomAlias; 
            }
            else if (string.IsNullOrWhiteSpace(dto.CustomAlias) && !string.IsNullOrWhiteSpace(entity.CustomAlias))
            {
                // User removed custom alias => Revert to generated code
                entity.CustomAlias = null;
                entity.ShortCode = ShortCodeGenerator.Encode(entity.ShortUrlId);
            }
            
            entity.OriginalUrl = dto.OriginalUrl;
            entity.ExpiresAt = dto.ExpiresAt;

            if (dto.IsActive != null)
            {
                entity.IsActive = dto.IsActive.Value;
            }
                
            _shortUrlRepository.Update(entity);
            await _shortUrlRepository.SaveChangesAsync();

            return new Dto_ShortUrl
            {
                ShortUrlId = entity.ShortUrlId,
                OriginalUrl = entity.OriginalUrl,
                ShortCode = entity.ShortCode,
                CustomAlias = entity.CustomAlias,
                CreatedAt = entity.CreatedAt,
                ExpiresAt = entity.ExpiresAt,
                IsActive = entity.IsActive,
                ClickCount = entity.ClickCount,
                UserId = entity.UserId
            };
        }
    }
}
