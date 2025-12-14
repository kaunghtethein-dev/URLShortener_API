using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener_Shared.DTOs;

namespace URLShortener_Application.Interfaces.Services
{
    public  interface IClickAnalyticsService
    {
        Task<Dto_ClickSummary> GetSummaryAsync(int userId);
        Task<List<Dto_DailyClickCount>> GetLast7DaysActivityAsync(int userId);
        Task<List<Dto_TopPerformingUrl>> GetTopPerformingUrlsAsync(int userId, int limit);
        Task<List<Dto_DeviceTypeStats>> GetDeviceTypeStatsAsync(int userId);
        Task<List<Dto_CountryAnalytics>> GetTopCountriesAsync(int userId, int limit);
    }
}
