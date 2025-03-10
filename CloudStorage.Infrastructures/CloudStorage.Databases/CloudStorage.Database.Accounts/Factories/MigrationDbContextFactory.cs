using CloudStorage.Database.Accounts.Contexts;
using CloudStorage.Database.Settings.Factories;

namespace CloudStorage.Database.Accounts.Factories;

public class MigrationDbContextFactory : MigrationDbContextFactoryBase<AccountsDbContext>
{
    public MigrationDbContextFactory() : base("Database")
    {
    }
    public override AccountsDbContext CreateDbContext(string[] args) => new(GetDbContextOptions());
}