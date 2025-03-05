using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pinterest.Domain.Posts.Entities;

namespace Pinterest.Database.Posts.Configurations;

public class TagsConfiguration : IEntityTypeConfiguration<TagInfo>
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