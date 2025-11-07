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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.UserId);
            builder.Property(x => x.UserName).IsRequired().HasMaxLength(100).IsUnicode(false);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Password).IsRequired().HasMaxLength(300);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.IsActive).HasDefaultValue(true);

            //Relationship
            builder.HasMany(x => x.ShortUrls)
                   .WithOne(s => s.User)
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
