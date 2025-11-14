using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            string shortCode;
            if (!string.IsNullOrWhiteSpace(dto.CustomAlias))
            {
                var existing = await _shortUrlRepository.GetByShortCodeAsync(dto.CustomAlias);
                if (existing != null)
                    throw new Exception("Custom alias is already in use.");

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
    }
}
