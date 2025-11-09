using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener_Domain.Entities;

namespace URLShortener_Application.Interfaces.Repositories
{
    public interface IClickAnalyticsRepository
    {
        Task<ClickAnalytics?> GetByIdAsync(long id);
        Task<IEnumerable<ClickAnalytics>> GetByShortUrlIdAsync(long shortUrlId);
        Task AddAsync(ClickAnalytics clickAnalytics);
        Task<int> SaveChangesAsync();

    }
}
