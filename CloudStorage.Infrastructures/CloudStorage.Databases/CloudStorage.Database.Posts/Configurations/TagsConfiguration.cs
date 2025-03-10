using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CloudStorage.Domain.Posts.Entities;

namespace CloudStorage.Database.Posts.Configurations;

internal class TagsConfiguration : IEntityTypeConfiguration<TagInfo>
{
    public void Configure(EntityTypeBuilder<TagInfo> builder)
    {
        builder.ToTable(nameof(TagInfo), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();
        
        builder.Property(item => item.Name).HasMaxLength(50).IsRequired();
        builder.HasMany(item => item.Posts).WithMany(item => item.Tags)
            .UsingEntity(item => item.ToTable("PostTagsConnection"));
    }
}