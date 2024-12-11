using System.Formats.Asn1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pinterest.Domain.Core.Models;
using Pinterest.Domain.Users.Entities;
using Pinterest.Domain.Users.Enums;

namespace Pinterest.Database.Users.Configurations;

public class UsersConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();
        builder.HasIndex(item => item.Email).IsUnique();
        
        builder.Property(item => item.Username).HasMaxLength(50).IsRequired();
        builder.Property(item => item.Email).HasMaxLength(100).IsRequired();
        builder.Property(item => item.Status).HasConversion(
            item => item.ToString(),
            item => (EntityStatus)Enum.Parse(typeof(EntityStatus), item));
        builder.Property(item => item.Gender).HasConversion(
            item => item.ToString(),
            item => (Gender)Enum.Parse(typeof(Gender), item));
        builder.HasMany(item => item.FavoritesPosts)
            .WithOne()
            .HasForeignKey("UserUuid")
            .HasPrincipalKey(item => item.Uuid)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(item => item.UserThemes)
            .WithOne()
            .HasForeignKey("UserUuid")
            .HasPrincipalKey(item => item.Uuid)
            .OnDelete(DeleteBehavior.Cascade);
    }
}