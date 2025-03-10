using CloudStorage.Database.Settings.Factories;
using CloudStorage.Database.Users.Contexts;

namespace CloudStorage.Database.Users.Factories;

public class MigrationDbContextFactory : MigrationDbContextFactoryBase<UsersDbContext>
{
    public MigrationDbContextFactory() : base("Database")
    {
    }
    public override UsersDbContext CreateDbContext(string[] args) => new(GetDbContextOptions());
}