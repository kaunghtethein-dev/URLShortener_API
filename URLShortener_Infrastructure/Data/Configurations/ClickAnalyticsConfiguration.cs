using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener_Domain.Entities;

namespace URLShortener_Infrastructure.Data.Configurations
{
    public class ClickAnalyticsConfiguration : IEntityTypeConfiguration<ClickAnalytics>
    {
        public void Configure(EntityTypeBuilder<ClickAnalytics> builder)
        {
            builder.ToTable("ClickAnalytics");

            builder.HasKey(x => x.ClickId);

            builder.Property(x => x.ClickedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.Property(x => x.IpAddress).HasMaxLength(100);

            builder.Property(x => x.UserAgent).HasMaxLength(100);

            builder.Property(x => x.Referrer).HasMaxLength(100);

            builder.Property(x => x.Country)
                .HasMaxLength(100);

            builder.Property(x => x.DeviceType)
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(x => x.ShortUrl)
                   .WithMany(s => s.ClickAnalytics)
                   .HasForeignKey(x => x.ShortUrlId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
