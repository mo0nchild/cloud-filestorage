using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pinterest.Domain.Posts.Entities;

namespace Pinterest.Database.Posts.Configurations;

public class PostsConfiguration : IEntityTypeConfiguration<PostInfo>
{
    public void Configure(EntityTypeBuilder<PostInfo> builder)
    {
        builder.ToTable(nameof(PostInfo), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();
        builder.Property(item => item.Title).HasMaxLength(100).IsRequired();
    }
}