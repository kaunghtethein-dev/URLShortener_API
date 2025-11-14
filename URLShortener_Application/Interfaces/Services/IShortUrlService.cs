using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener_Shared.DTOs;

namespace URLShortener_Application.Interfaces.Services
{
    public interface IShortUrlService
    {
        Task<Dto_ShortUrl> CreateShortUrlAsync(Dto_CreateShortUrl dto);
        Task<Dto_ShortUrl?> GetByShortCodeAsync(string shortCode);
        Task<IEnumerable<Dto_ShortUrl>> GetByUserIdAsync(int userId);
    }
}
