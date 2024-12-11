using Pinterest.Database.Settings.Factories;
using Pinterest.Database.Users.Context;

namespace Pinterest.Database.Users.Factories;

public class MigrationDbContextFactory : MigrationDbContextFactoryBase<UsersDbContext>
{
    public MigrationDbContextFactory() : base("Database")
    { }
    public override UsersDbContext CreateDbContext(string[] args) => new(this.GetDbContextOptions());
}