using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Domain.Entities
{
    public class ClickAnalytics
    {
        public long ClickId { get; set; }
        public long ShortUrlId { get; set; }
        public DateTime ClickedAt { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Referrer { get; set; }
        public string? Country { get; set; }
        public string? DeviceType { get; set; }

        public ShortUrl? ShortUrl { get; set; }

    }
}
