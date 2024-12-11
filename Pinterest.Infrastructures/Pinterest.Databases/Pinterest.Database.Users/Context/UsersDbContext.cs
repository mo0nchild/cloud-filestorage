using Microsoft.EntityFrameworkCore;
using Pinterest.Application.Users.Repositories;
using Pinterest.Database.Users.Configurations;
using Pinterest.Domain.Users.Entities;

namespace Pinterest.Database.Users.Context;

public class UsersDbContext : DbContext, IUserDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserTheme> UserThemes { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<FavoritePost> FavoritePosts { get; set; }

    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder.UseLazyLoadingProxies());
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UsersConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
    }
}