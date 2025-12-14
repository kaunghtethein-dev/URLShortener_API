using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener_Domain.Entities;

namespace URLShortener_Application.Interfaces.Repositories
{
    public interface IShortUrlRepository
    {
        Task<ShortUrl?> GetByIdAsync(long id);
        Task<ShortUrl?> GetByShortCodeAsync(string shortCode);
        Task<IEnumerable<ShortUrl>> GetByUserIdAsync(int userId);
        Task AddAsync(ShortUrl shortUrl);
        void Update(ShortUrl shortUrl);
        void Delete(ShortUrl shortUrl);
        Task<long> CountByUserAsync(int userId);
        Task<long> CountActiveByUserAsync(int userId);
        Task<long> CountExpiredByUserAsync(int userId);
        Task<List<ShortUrl>> GetTopPerformingByUserAsync(int userId,int limit);
        Task<int> SaveChangesAsync();

    }
}
