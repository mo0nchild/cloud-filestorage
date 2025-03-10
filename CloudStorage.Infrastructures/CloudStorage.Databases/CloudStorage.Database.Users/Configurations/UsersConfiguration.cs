using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CloudStorage.Domain.Users.Entities;

namespace CloudStorage.Database.Users.Configurations;

internal class UsersConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();
        
        builder.Property(item => item.Email).HasMaxLength(100).IsRequired();
        builder.Property(item => item.Username).HasMaxLength(50).IsRequired();
        
        builder.Property(item => item.UserThemes)
            .HasConversion(
                value => JsonSerializer.Serialize(value, new JsonSerializerOptions()),
                value => JsonSerializer.Deserialize<List<string>>(value, new JsonSerializerOptions())!
            )
            .HasColumnType("json");
    }
}