using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pinterest.Application.Users.Repositories;
using Pinterest.Database.Settings.Factories;
using Pinterest.Database.Settings.Helpers;
using Pinterest.Database.Users.Context;

namespace Pinterest.Database.Users;

public static class Bootstrapper
{
    private static readonly string DbSettingsSection = "Database";
    public static async Task<IServiceCollection> AddUsersDatabase(this IServiceCollection collection, 
        IConfiguration configuration)
    {
        var settings = collection.Configure<UsersDbContextSettings>(configuration.GetSection(DbSettingsSection))
            .BuildServiceProvider()
            .GetRequiredService<IOptions<UsersDbContextSettings>>();
        collection.AddDbContextFactory<UsersDbContext>(options =>
        {
            DbContextOptionsFactory<UsersDbContext>.Configure(settings.Value.ConnectionString, true).Invoke(options);
        });
        await collection.AddDbContextFactoryWrapper<IUserDbContext, UsersDbContext>();
        
        var serviceProvider = collection.BuildServiceProvider();
        var dbContextFactory = serviceProvider.GetService<IDbContextFactory<UsersDbContext>>()!;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.Database.MigrateAsync();
        return collection;
    }
}