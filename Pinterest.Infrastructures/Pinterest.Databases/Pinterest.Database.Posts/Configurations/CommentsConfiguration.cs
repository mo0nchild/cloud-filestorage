using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pinterest.Domain.Posts.Entities;

namespace Pinterest.Database.Posts.Configurations;

internal class CommentsConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable(nameof(Comment), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();
        builder.Property(item => item.Text).HasMaxLength(255).IsRequired();
        builder.Property(item => item.UserUuid).IsRequired();
        
        builder.HasOne(item => item.Post).WithMany(item => item.Comments)
            .HasPrincipalKey(item => item.Uuid)
            .HasForeignKey("PostId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(item => item.ParentComment).WithMany(item => item.SubComments)
            .HasPrincipalKey(item => item.Uuid)
            .HasForeignKey("ParentCommentId")
            .OnDelete(DeleteBehavior.SetNull);
    }
}