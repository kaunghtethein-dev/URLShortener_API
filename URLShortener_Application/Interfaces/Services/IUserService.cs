using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener_Shared.DTOs;

namespace URLShortener_Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<Dto_User?> GetUserByIdAsync(int id);
        Task<IEnumerable<Dto_User>> GetAllUsersAsync();
        Task<Dto_User> CreateUserAsync(Dto_CreateUser dto);
        Task<bool> DeleteUserAsync(int id);
        Task<string?> LoginUserAsync(Dto_LoginUser dto);
        Task<Dto_User?> GetUserByEmailAsync(string email);


    }
}
