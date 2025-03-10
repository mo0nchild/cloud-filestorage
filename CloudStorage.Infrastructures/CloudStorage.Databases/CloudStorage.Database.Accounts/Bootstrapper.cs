using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using CloudStorage.Application.Accounts.Repositories;
using CloudStorage.Database.Accounts.Contexts;
using CloudStorage.Database.Settings.Factories;
using CloudStorage.Database.Settings.Helpers;

namespace CloudStorage.Database.Accounts;

public static class Bootstrapper
{
    private static readonly string DbSettingsSection = "Database";
    public static async Task<IServiceCollection> AddAccountsDatabase(this IServiceCollection serviceCollection, 
        IConfiguration configuration)
    {
        var settings = serviceCollection.Configure<AccountsDbContextSettings>(configuration.GetSection(DbSettingsSection))
            .BuildServiceProvider()
            .GetRequiredService<IOptions<AccountsDbContextSettings>>();
        serviceCollection.AddDbContextFactory<AccountsDbContext>(options =>
        {
            DbContextOptionsFactory<AccountsDbContext>.Configure(settings.Value.ConnectionString, true).Invoke(options);
        });
        await serviceCollection.AddDbContextFactoryWrapper<IAccountsRepository, AccountsDbContext>();
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContextFactory = serviceProvider.GetService<IDbContextFactory<AccountsDbContext>>()!;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.Database.MigrateAsync();
        return serviceCollection;
    }
}