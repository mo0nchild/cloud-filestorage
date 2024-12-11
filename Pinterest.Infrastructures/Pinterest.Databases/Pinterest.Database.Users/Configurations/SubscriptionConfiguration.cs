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

        builder.HasOne(item => item.Subscriber)
            .WithMany(item => item.Subscribers)
            .HasForeignKey("SubscriberUuid")
            .HasPrincipalKey(item => item.Uuid)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(item => item.Subscriptions)
            .WithMany(item => item.Subscriptions)
            .HasForeignKey("SubscriptionsUuid")
            .HasPrincipalKey(item => item.Uuid)
            .OnDelete(DeleteBehavior.Cascade);
    }
}