using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using CloudStorage.Application.Accounts.Repositories;
using CloudStorage.Database.Accounts.Configurations;
using CloudStorage.Domain.Authorization.Entities;

namespace CloudStorage.Database.Accounts.Contexts;

public class AccountsDbContext(DbContextOptions<AccountsDbContext> options) : DbContext(options), IAccountsRepository
{
    public virtual DbSet<AccountInfo> AccountInfos { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder.UseLazyLoadingProxies());
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new AccountsConfiguration());
    }
    public Task<IDbContextTransaction> BeginTransactionAsync() => Database.BeginTransactionAsync();
    public IDbContextTransaction BeginTransaction() => Database.BeginTransaction();
}