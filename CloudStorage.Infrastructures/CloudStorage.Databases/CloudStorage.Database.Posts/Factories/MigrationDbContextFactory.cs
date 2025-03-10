using CloudStorage.Database.Posts.Contexts;
using CloudStorage.Database.Settings.Factories;

namespace CloudStorage.Database.Posts.Factories;

public class MigrationDbContextFactory : MigrationDbContextFactoryBase<PostsDbContext>
{
    public MigrationDbContextFactory() : base("Database")
    {
    }
    public override PostsDbContext CreateDbContext(string[] args) => new(GetDbContextOptions());
}