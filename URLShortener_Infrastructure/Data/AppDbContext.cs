using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener_Domain.Entities;
using URLShortener_Infrastructure.Data.Configurations;

namespace URLShortener_Infrastructure.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ShortUrl> ShortUrls { get; set; }

        public DbSet<ClickAnalytics> ClickAnalytics { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ShortUrlConfiguration());
            modelBuilder.ApplyConfiguration(new ClickAnalyticsConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        }

    }
}
