using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener_Application.Interfaces.Repositories;
using URLShortener_Application.Interfaces.Services;
using URLShortener_Application.Services.Auth;
using URLShortener_Application.Services.Helpers;
using URLShortener_Application.Settings;
using URLShortener_Domain.Entities;
using URLShortener_Shared.DTOs;

namespace URLShortener_Application.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly JwtSettings _jwtSettings;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, JwtTokenGenerator jwtTokenGenerator, IRefreshTokenRepository refreshTokenRepository, IOptions<JwtSettings> jwtOptions)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = new PasswordHasher<User>();
            _refreshTokenRepository = refreshTokenRepository;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<Dto_User?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return new Dto_User
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                IsActive = user.IsActive
            };
        }

        public async Task<IEnumerable<Dto_User>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new Dto_User
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                IsActive = u.IsActive
            });
        }

        public async Task<Dto_User> CreateUserAsync(Dto_CreateUser dto)
        {
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                IsActive = true
            };
            user.Password = _passwordHasher.HashPassword(user,dto.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return new Dto_User
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                IsActive = user.IsActive
            };
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            _userRepository.Delete(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }
        public async Task<Dto_User?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            return new Dto_User
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                IsActive = user.IsActive
            };
        }
        public async Task<bool> CheckUserAlreadyExists(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null; //return true if there is already a user with the email
        }
        public async Task<Dto_AuthResponse?> LoginUserAsync(Dto_LoginUser dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            // Generate access token
            var accessToken = _jwtTokenGenerator.GenerateToken(user);
            var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpInMinutes);

            // Generate refresh token 
            var rawRefreshToken = TokenHelpers.GenerateRandomToken();
            var hashed = TokenHelpers.ComputeSha256Hash(rawRefreshToken);

            var refreshEntity = new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = hashed,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpDays),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            await _refreshTokenRepository.AddAsync(refreshEntity);
            await _refreshTokenRepository.SaveChangesAsync();

            return new Dto_AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = rawRefreshToken,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<Dto_AuthResponse?> RefreshTokenAsync(string refreshToken)
        {
            var hash = TokenHelpers.ComputeSha256Hash(refreshToken);
            var tokenEntity = await _refreshTokenRepository.GetByTokenHashAsync(hash);

            if (tokenEntity == null)
            {
                return null;
            }

            if (tokenEntity.IsRevoked || tokenEntity.ExpiresAt <= DateTime.UtcNow)
            {
                return null;
            }

            var user = tokenEntity.User;
            if (user == null)
            {
                user = await _userRepository.GetByIdAsync(tokenEntity.UserId);
                if (user == null)
                {
                    return null;
                }
            }

            // Revoke current token
            tokenEntity.IsRevoked = true;
            _refreshTokenRepository.Update(tokenEntity);

            // Create new refresh token entity 
            var newRaw = TokenHelpers.GenerateRandomToken();
            var newHash = TokenHelpers.ComputeSha256Hash(newRaw);

            var newTokenEntity = new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = newHash,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpDays),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            await _refreshTokenRepository.AddAsync(newTokenEntity);
            await _refreshTokenRepository.SaveChangesAsync();

            // generate new access token
            var newAccessToken = _jwtTokenGenerator.GenerateToken(user);
            var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpInMinutes);

            var response = new Dto_AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRaw,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                UserName = user.UserName,
                Email = user.Email
            };

            return response;
        }
    }
}
