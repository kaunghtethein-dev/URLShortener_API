using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Application.Services.Helpers
{
    public static class UserAgentHelper
    {
        public static string GetUserAgentBrowser(string? userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return "Unknown";
            }
            userAgent = userAgent.ToLower();
            string browser = userAgent.Contains("chrome") && !userAgent.Contains("edge") ? "Chrome" :
            userAgent.Contains("edg") ? "Edge" :
            userAgent.Contains("firefox") ? "Firefox" :
            userAgent.Contains("safari") && !userAgent.Contains("chrome") ? "Safari" :
            userAgent.Contains("opera") || userAgent.Contains("opr") ? "Opera" :
            "Unknown";
            return browser;
        }
    }
}
