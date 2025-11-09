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
    public class ClickAnalyticsRepository: IClickAnalyticsRepository
    {
        private readonly AppDbContext _context;
        public ClickAnalyticsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ClickAnalytics?> GetByIdAsync(long id)
        {
            return await _context.ClickAnalytics.FindAsync(id);
        }

        public async Task<IEnumerable<ClickAnalytics>> GetByShortUrlIdAsync(long shortUrlId)
        {
            return await _context.ClickAnalytics
                .Where(c => c.ShortUrlId == shortUrlId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(ClickAnalytics clickAnalytics)
        {
            await _context.ClickAnalytics.AddAsync(clickAnalytics);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
