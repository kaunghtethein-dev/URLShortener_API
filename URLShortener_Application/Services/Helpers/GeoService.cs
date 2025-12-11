using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener_Application.Interfaces.Services.Helpers;

namespace URLShortener_Application.Services.Helpers
{
    public class GeoService:IGeoService
    {
        private readonly HttpClient _httpClient;

        public GeoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> ResolveCountryAsync(string? ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                return null;
            }
                
            try
            {
                var url = $"https://ipapi.co/{ip}/country_name/";
                var result = await _httpClient.GetStringAsync(url);

                if (string.IsNullOrWhiteSpace(result))
                    return null;

                return result.Trim();
            }
            catch
            {
                return null; 
            }
        }
    }
}
