using Microsoft.EntityFrameworkCore;
using Pinterest.Application.Accounts.Repositories;
using Pinterest.Database.Accounts.Configurations;
using Pinterest.Domain.Authorization.Entities;

namespace Pinterest.Database.Accounts.Contexts;

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
}