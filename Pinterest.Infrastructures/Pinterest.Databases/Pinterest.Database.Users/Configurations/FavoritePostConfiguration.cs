using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pinterest.Domain.Users.Entities;

namespace Pinterest.Database.Users.Configurations;

internal class FavoritePostConfiguration : IEntityTypeConfiguration<FavoritePost>
{
    public void Configure(EntityTypeBuilder<FavoritePost> builder)
    {
        builder.ToTable(nameof(FavoritePost), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();

        builder.Property(item => item.PostUuid).IsRequired();
        builder.HasOne(item => item.User).WithMany(item => item.FavoritesPosts)
            .HasPrincipalKey(item => item.Uuid)
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}