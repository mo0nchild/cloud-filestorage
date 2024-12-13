using Pinterest.Database.Accounts.Contexts;
using Pinterest.Database.Settings.Factories;

namespace Pinterest.Database.Accounts.Factories;

public class MigrationDbContextFactory : MigrationDbContextFactoryBase<AccountsDbContext>
{
    public MigrationDbContextFactory() : base("Database")
    {
    }
    public override AccountsDbContext CreateDbContext(string[] args) => new(GetDbContextOptions());
}