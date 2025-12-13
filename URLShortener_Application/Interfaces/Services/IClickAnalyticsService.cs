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
    }
}
