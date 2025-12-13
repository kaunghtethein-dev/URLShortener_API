using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener_Application.Interfaces.Repositories;
using URLShortener_Application.Interfaces.Services;
using URLShortener_Shared.DTOs;

namespace URLShortener_Application.Services
{
    public class ClickAnalyticsService : IClickAnalyticsService
    {
        private readonly IShortUrlRepository _shortUrlRepository;
        private readonly IClickAnalyticsRepository _clickRepository;

        public ClickAnalyticsService(IShortUrlRepository shortUrlRepository,
        IClickAnalyticsRepository clickRepository) 
        {
            _shortUrlRepository = shortUrlRepository;
            _clickRepository = clickRepository;
        }
        public async Task<Dto_ClickSummary> GetSummaryAsync(int userId)
        {
            var totalShortUrls = await _shortUrlRepository.CountByUserAsync(userId);
            var totalClicks = await _clickRepository.GetTotalClicksByUserAsync(userId);
            var activeCount = await _shortUrlRepository.CountActiveByUserAsync(userId);
            var expiredCount = await _shortUrlRepository.CountExpiredByUserAsync(userId);

            double average = totalShortUrls == 0
                ? 0
                : (double)totalClicks / totalShortUrls;

            return new Dto_ClickSummary
            {
                TotalShortUrls = totalShortUrls,
                TotalClicks = totalClicks,
                AverageClicks = Math.Round(average, 2),
                TotalActive = activeCount,
                TotalExpired = expiredCount
            };
        }
    }
}
