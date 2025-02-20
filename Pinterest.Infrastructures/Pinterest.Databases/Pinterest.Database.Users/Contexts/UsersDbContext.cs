using Microsoft.EntityFrameworkCore;
using Pinterest.Application.Users.Repositories;
using Pinterest.Database.Users.Configurations;
using Pinterest.Domain.Users.Entities;

namespace Pinterest.Database.Users.Contexts;

public class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options), IUsersRepository
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<FavoritePost> FavoritePosts { get; set; }
    public virtual DbSet<Subscription> Subscriptions { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder.UseLazyLoadingProxies());
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UsersConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
        modelBuilder.ApplyConfiguration(new FavoritePostConfiguration());
    }
}