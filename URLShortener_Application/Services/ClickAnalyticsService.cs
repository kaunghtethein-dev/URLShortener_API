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
        public async Task<List<Dto_DailyClickCount>> GetLast7DaysActivityAsync(int userId)
        {
            var todayUtc = DateTime.UtcNow.Date;
            var startDate = todayUtc.AddDays(-6); // inclusive
            var endDate = todayUtc.AddDays(1);    // exclusive

            var clicks = await _clickRepository.GetClicksForUserBetweenAsync(userId, startDate, endDate);

            var grouped = clicks.GroupBy(c => c.ClickedAt.Date).ToDictionary(g => g.Key, g => g.LongCount());

            var result = new List<Dto_DailyClickCount>();

            for (int i = 0; i < 7; i++)
            {
                var date = startDate.AddDays(i);

                result.Add(new Dto_DailyClickCount
                {
                    Date = date,
                    ClickCount = grouped.ContainsKey(date)? grouped[date]: 0
                });
            }

            return result;
        }
    }

}
