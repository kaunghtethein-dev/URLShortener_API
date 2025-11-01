using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace URLShortener_API.Entities
{
    public class UrlShortener_Entities
    {
        public class User
        {
            [Key]
            public int UserId { get; set; }

            [Required, MaxLength(50)]
            public string UserName { get; set; } = string.Empty;

            [Required, MaxLength(100)]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Password { get; set; } = string.Empty;

            public DateTime CreatedAt { get; set; } = DateTime.Now;

            public DateTime? LastLoginAt { get; set; }

            public bool IsActive { get; set; } = true;

            // Navigation property
            public ICollection<ShortUrl> ShortUrls { get; set; } = new List<ShortUrl>();
        }
        public class ShortUrl
        {
            [Key]
            public long ShortUrlId { get; set; }

            public int? UserId { get; set; }

            [Required]
            public string OriginalUrl { get; set; } = string.Empty ;

            [Required, MaxLength(20)]
            public string ShortCode { get; set; } = string.Empty;

            [MaxLength(50)]
            public string CustomAlias { get; set; } = string.Empty;

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public DateTime? ExpiresAt { get; set; }

            public bool IsActive { get; set; } = true;

            public long ClickCount { get; set; } = 0;

            // Navigation properties
            [ForeignKey("UserId")]
            public User? User { get; set; }

            public ICollection<ClickAnalytics> ClickAnalytics { get; set; } = new List<ClickAnalytics>();
        }
        public class ClickAnalytics
        {
            [Key]
            public long ClickId { get; set; }

            [Required]
            public long ShortUrlId { get; set; }

            public DateTime ClickedAt { get; set; } = DateTime.UtcNow;

            [MaxLength(45)]
            public string IpAddress { get; set; } = string.Empty;

            [MaxLength(500)]
            public string UserAgent { get; set; } = string.Empty;

            [MaxLength(500)]
            public string Referrer { get; set; } = string.Empty;

            [MaxLength(100)]
            public string Country { get; set; } = string.Empty ;

            [MaxLength(50)]
            public string DeviceType { get; set; } = string.Empty;

            // Navigation property
            [ForeignKey("ShortUrlId")]
            public ShortUrl? ShortUrl { get; set; }
        }
    }
}
