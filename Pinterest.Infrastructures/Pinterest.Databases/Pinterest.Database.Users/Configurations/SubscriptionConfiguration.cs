using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pinterest.Domain.Users.Entities;

namespace Pinterest.Database.Users.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable(nameof(Subscription), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();
        
        builder.HasOne(item => item.Subscriber).WithMany(item => item.Subscriptions)
            .HasPrincipalKey(item => item.Uuid)
            .HasForeignKey("SubscriberId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(item => item.Author).WithMany(item => item.Subscribers)
            .HasPrincipalKey(item => item.Uuid)
            .HasForeignKey("AuthorId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}