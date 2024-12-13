using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pinterest.Application.Accounts.Repositories;
using Pinterest.Database.Accounts.Contexts;
using Pinterest.Database.Settings.Factories;
using Pinterest.Database.Settings.Helpers;

namespace Pinterest.Database.Accounts;

public static class Bootstrapper
{
    private static readonly string DbSettingsSection = "Database";
    public static async Task<IServiceCollection> AddAccountsDatabase(this IServiceCollection collection, 
        IConfiguration configuration)
    {
        var settings = collection.Configure<AccountsDbContextSettings>(configuration.GetSection(DbSettingsSection))
            .BuildServiceProvider()
            .GetRequiredService<IOptions<AccountsDbContextSettings>>();
        collection.AddDbContextFactory<AccountsDbContext>(options =>
        {
            DbContextOptionsFactory<AccountsDbContext>.Configure(settings.Value.ConnectionString, true).Invoke(options);
        });
        await collection.AddDbContextFactoryWrapper<IAccountsRepository, AccountsDbContext>();
        
        var serviceProvider = collection.BuildServiceProvider();
        var dbContextFactory = serviceProvider.GetService<IDbContextFactory<AccountsDbContext>>()!;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.Database.MigrateAsync();
        return collection;
    }
}