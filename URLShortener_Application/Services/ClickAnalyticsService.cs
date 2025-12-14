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
        public async Task<List<Dto_TopPerformingUrl>> GetTopPerformingUrlsAsync(int userId,int limit = 3)
        {
            var urls = await _shortUrlRepository
                .GetTopPerformingByUserAsync(userId, limit);

            var result = new List<Dto_TopPerformingUrl>();

            foreach (var url in urls)
            {
                result.Add(new Dto_TopPerformingUrl
                {
                    ShortUrlId = url.ShortUrlId,
                    ShortCode = url.ShortCode,
                    OriginalUrl = url.OriginalUrl,
                    ClickCount = url.ClickCount
                });
            }

            return result;
        }
        public async Task<List<Dto_DeviceTypeStats>> GetDeviceTypeStatsAsync(int userId)
        {
            var data = await _clickRepository.GetClicksByDeviceTypeAsync(userId);

            var result = new List<Dto_DeviceTypeStats>();

            foreach (var item in data)
            {
                result.Add(new Dto_DeviceTypeStats
                {
                    DeviceType = item.DeviceType,
                    ClickCount = item.ClickCount
                });
            }

            return result;
        }
        public async Task<List<Dto_CountryAnalytics>> GetTopCountriesAsync(int userId, int limit = 5)
        {
            var data = await _clickRepository.GetClickCountsByCountryAsync(userId);

            var topCountries = data
                .Take(limit)
                .Select(x => new Dto_CountryAnalytics
                {
                    Country = x.Country,
                    Clicks = x.ClickCount
                })
                .ToList();

            var othersClicks = data.Skip(limit).Sum(x => x.ClickCount);

            if (othersClicks > 0)
            {
                topCountries.Add(new Dto_CountryAnalytics
                {
                    Country = "Others",
                    Clicks = othersClicks
                });
            }

            return topCountries;
        }


    }

}
