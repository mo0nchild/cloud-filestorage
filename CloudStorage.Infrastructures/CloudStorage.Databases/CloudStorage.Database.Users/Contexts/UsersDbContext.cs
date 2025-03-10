using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using CloudStorage.Application.Users.Repositories;
using CloudStorage.Database.Users.Configurations;
using CloudStorage.Domain.Users.Entities;

namespace CloudStorage.Database.Users.Contexts;

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
    public Task<IDbContextTransaction> BeginTransactionAsync() => Database.BeginTransactionAsync();
    public IDbContextTransaction BeginTransaction() => Database.BeginTransaction();
}