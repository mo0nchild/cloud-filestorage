using Pinterest.Database.Posts.Contexts;
using Pinterest.Database.Settings.Factories;

namespace Pinterest.Database.Posts.Factories;

public class MigrationDbContextFactory : MigrationDbContextFactoryBase<PostsDbContext>
{
    public MigrationDbContextFactory() : base("Database")
    {
    }
    public override PostsDbContext CreateDbContext(string[] args) => new(GetDbContextOptions());
}