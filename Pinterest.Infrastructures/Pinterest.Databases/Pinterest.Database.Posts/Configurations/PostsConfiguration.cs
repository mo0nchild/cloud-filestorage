using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pinterest.Domain.Posts.Entities;
using System.Text.Json;

namespace Pinterest.Database.Posts.Configurations;

public class PostsConfiguration : IEntityTypeConfiguration<PostInfo>
{
    public void Configure(EntityTypeBuilder<PostInfo> builder)
    {
        builder.ToTable(nameof(PostInfo), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();
        builder.Property(item => item.Title).HasMaxLength(100).IsRequired();
        
        builder.Property(item => item.Description).HasColumnType("text").IsRequired(false);
        builder.Property(item => item.AuthorUuid).IsRequired();
        builder.Property(item => item.FileUuid).IsRequired();
        builder.Property(item => item.PreviewUuid).IsRequired(false);
        
        builder.Property(item => item.GrantedAccess)
            .HasConversion(
                value => JsonSerializer.Serialize(value, new JsonSerializerOptions()),
                value => JsonSerializer.Deserialize<List<Guid>>(value, new JsonSerializerOptions())!
            ).HasColumnType("json");
    }
}