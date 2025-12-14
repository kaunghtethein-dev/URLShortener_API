using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener_Application.Interfaces.Repositories;
using URLShortener_Application.Services.Helpers;
using URLShortener_Domain.Entities;
using URLShortener_Infrastructure.Data;
using URLShortener_Shared.DTOs;

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
        public Task<long> GetTotalClicksByUserAsync(int userId)
        {
            return _context.ClickAnalytics
                .Where(c => c.ShortUrl!.UserId == userId)
                .LongCountAsync();
        }
        public async Task<List<ClickAnalytics>> GetClicksForUserBetweenAsync(int userId,DateTime fromUtc,DateTime toUtc)
        {
            return await _context.ClickAnalytics
                .Where(c =>
                    c.ShortUrl!.UserId == userId &&
                    c.ClickedAt >= fromUtc &&
                    c.ClickedAt < toUtc)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<DeviceTypeCount>> GetClicksByDeviceTypeAsync(int userId)
        {
            return await _context.ClickAnalytics
                .Where(c => c.ShortUrl!.UserId == userId)
                .GroupBy(c => c.DeviceType)
                .Select(g => new DeviceTypeCount
                {
                    DeviceType = g.Key!,
                    ClickCount = g.LongCount()
                })
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<CountryClickCount>> GetClickCountsByCountryAsync(int userId)
        {
            return await _context.ClickAnalytics
                .Where(c =>
                    c.ShortUrl!.UserId == userId &&
                    c.Country != null &&
                    c.Country != "")
                .GroupBy(c => c.Country!)
                .Select(g => new CountryClickCount
                {
                    Country = g.Key,
                    ClickCount = g.LongCount()
                })
                .OrderByDescending(x => x.ClickCount)
                .ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
