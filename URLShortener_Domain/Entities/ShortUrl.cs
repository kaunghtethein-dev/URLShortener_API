using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Domain.Entities
{
    public class ShortUrl
    {
        public long ShortUrlId { get; set; }
        public int? UserId { get; set; }
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string? CustomAlias { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
        public long ClickCount { get; set; } = 0;

        public User? User { get; set; }
        public ICollection<ClickAnalytics> ClickAnalytics { get; set; } = new List<ClickAnalytics>();

    }
}
