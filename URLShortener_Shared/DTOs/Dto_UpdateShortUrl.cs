using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Shared.DTOs
{
    public class Dto_UpdateShortUrl
    {
        public long ShortUrlId { get; set; }
        public string OriginalUrl { get; set; } = string.Empty;
        public string? CustomAlias { get; set; }
        public DateTime? ExpiresAt { get; set; } = null;
        public bool? IsActive { get; set; }
    }
}
