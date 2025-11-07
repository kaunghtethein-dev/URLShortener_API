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
    public class ShortUrlConfiguration : IEntityTypeConfiguration<ShortUrl>
    {
        public void Configure(EntityTypeBuilder<ShortUrl> builder)
        {
            builder.ToTable("ShortUrls");
            builder.HasKey(x=>x.ShortUrlId);
            builder.Property(x => x.OriginalUrl).IsRequired().HasMaxLength(2048);

            builder.Property(x => x.ShortCode).IsRequired().HasMaxLength(20);

            builder.Property(x => x.CustomAlias).HasMaxLength(50);

            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.IsActive).HasDefaultValue(true);

            builder.HasIndex(x => x.ShortCode).IsUnique();

            // Relationships
            builder.HasOne(x => x.User)
                   .WithMany(u => u.ShortUrls)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.ClickAnalytics)
                   .WithOne(c => c.ShortUrl)
                   .HasForeignKey(c => c.ShortUrlId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
