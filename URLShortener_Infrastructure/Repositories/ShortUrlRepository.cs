using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener_Application.Interfaces.Repositories;
using URLShortener_Domain.Entities;
using URLShortener_Infrastructure.Data;

namespace URLShortener_Infrastructure.Repositories
{
    public class ShortUrlRepository: IShortUrlRepository
    {
        private readonly AppDbContext _context;
        public ShortUrlRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ShortUrl?> GetByIdAsync(long id)
        {
            return await _context.ShortUrls
                .Include(s => s.User)
                .Include(s => s.ClickAnalytics)
                .FirstOrDefaultAsync(s => s.ShortUrlId == id);
        }

        public async Task<ShortUrl?> GetByShortCodeAsync(string shortCode)
        {
            return await _context.ShortUrls
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.ShortCode == shortCode);
        }

        public async Task<IEnumerable<ShortUrl>> GetByUserIdAsync(int userId)
        {
            return await _context.ShortUrls
                .Where(s => s.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(ShortUrl shortUrl)
        {
            await _context.ShortUrls.AddAsync(shortUrl);
        }

        public void Update(ShortUrl shortUrl)
        {
            _context.ShortUrls.Update(shortUrl);
        }

        public void Delete(ShortUrl shortUrl)
        {
            _context.ShortUrls.Remove(shortUrl);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
