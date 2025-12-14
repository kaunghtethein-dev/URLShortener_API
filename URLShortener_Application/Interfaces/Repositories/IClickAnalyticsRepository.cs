using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener_Application.Services.Helpers;
using URLShortener_Domain.Entities;
using URLShortener_Shared.DTOs;


namespace URLShortener_Application.Interfaces.Repositories
{
    public interface IClickAnalyticsRepository
    {
        Task<ClickAnalytics?> GetByIdAsync(long id);
        Task<IEnumerable<ClickAnalytics>> GetByShortUrlIdAsync(long shortUrlId);
        Task AddAsync(ClickAnalytics clickAnalytics);
        Task<long> GetTotalClicksByUserAsync(int userId);
        Task<List<ClickAnalytics>> GetClicksForUserBetweenAsync(int userId,DateTime fromUtc,DateTime toUtc);
        Task<List<DeviceTypeCount>> GetClicksByDeviceTypeAsync(int userId);
        Task<List<CountryClickCount>> GetClickCountsByCountryAsync(int userId);
        Task<int> SaveChangesAsync();

    }
}
