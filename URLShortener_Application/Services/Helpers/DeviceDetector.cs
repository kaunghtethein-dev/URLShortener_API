using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Application.Services.Helpers
{
    public static class DeviceDetector
    {
        public static string Detect(string? ua)
        {
            if (ua == null) return "Unknown";

            ua = ua.ToLower();

            if (ua.Contains("mobile")) return "Mobile";
            if (ua.Contains("tablet")) return "Tablet";
            if (ua.Contains("windows") ||
                ua.Contains("macintosh") ||
                ua.Contains("linux")) return "Desktop";

            return "Unknown";
        }
    }
}
