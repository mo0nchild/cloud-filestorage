﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using CloudStorage.Application.Users.Repositories;
using CloudStorage.Database.Settings.Factories;
using CloudStorage.Database.Settings.Helpers;
using CloudStorage.Database.Users.Contexts;

namespace CloudStorage.Database.Users;

public static class Bootstrapper
{
    private static readonly string DbSettingsSection = "Database";
    
    public static async Task<IServiceCollection> AddUsersDatabase(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var settings = serviceCollection.Configure<UsersDbContextSettings>(configuration.GetSection(DbSettingsSection))
            .BuildServiceProvider()
            .GetRequiredService<IOptions<UsersDbContextSettings>>();
        serviceCollection.AddDbContextFactory<UsersDbContext>(options =>
        {
            DbContextOptionsFactory<UsersDbContext>.Configure(settings.Value.ConnectionString, true).Invoke(options);
        });
        await serviceCollection.AddDbContextFactoryWrapper<IUsersRepository, UsersDbContext>();
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContextFactory = serviceProvider.GetService<IDbContextFactory<UsersDbContext>>()!;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.Database.MigrateAsync();
        return serviceCollection;
    }
}