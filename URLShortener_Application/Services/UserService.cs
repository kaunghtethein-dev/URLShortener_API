using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener_Application.Interfaces.Repositories;
using URLShortener_Application.Interfaces.Services;
using URLShortener_Application.Interfaces.Services.Auth;
using URLShortener_Domain.Entities;
using URLShortener_Shared.DTOs;

namespace URLShortener_Application.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public UserService(IUserRepository userRepository, JwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
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
                Password = dto.Password, // TODO: hash later
                IsActive = true
            };

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
        public async Task<string?> LoginUserAsync(Dto_LoginUser dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null || user.Password != dto.Password)
                return null;


            return _jwtTokenGenerator.GenerateToken(user);
        }
    }
}
