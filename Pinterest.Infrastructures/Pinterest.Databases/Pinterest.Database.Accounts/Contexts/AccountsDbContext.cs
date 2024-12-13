using Microsoft.EntityFrameworkCore;
using Pinterest.Application.Accounts.Repositories;
using Pinterest.Database.Accounts.Configurations;
using Pinterest.Domain.Authorization.Entities;

namespace Pinterest.Database.Accounts.Contexts;

public class AccountsDbContext : DbContext, IAccountsRepository
{
    public DbSet<AccountInfo> AccountInfos { get; set; }
    
    public AccountsDbContext(DbContextOptions<AccountsDbContext> options) : base(options) { }
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