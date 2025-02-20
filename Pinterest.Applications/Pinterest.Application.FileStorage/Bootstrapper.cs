using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pinterest.Application.FileStorage.Infrastructures.Interfaces;
using Pinterest.Application.FileStorage.Interfaces;
using Pinterest.Application.FileStorage.Services;
using Pinterest.Domain.FileStorage.Settings;

namespace Pinterest.Application.FileStorage;

public static class Bootstrapper
{
    private static readonly string StorageSettingsSection = "StorageSettings";
    public static async Task<IServiceCollection> AddFileStorageServices(this IServiceCollection serviceCollection, 
        IConfiguration configuration)
    {
        serviceCollection.Configure<StorageSettings>(configuration.GetSection(StorageSettingsSection));
        serviceCollection.AddTransient<IFileStorageService, FileStorageService>();
        serviceCollection.AddTransient<IFileStorageValidators, FileStorageValidators>();
        
        using var serviceScope = serviceCollection.BuildServiceProvider().CreateScope();
        var bucketManager = serviceScope.ServiceProvider.GetRequiredService<IBucketManager>();
        var options = serviceScope.ServiceProvider.GetRequiredService<IOptions<StorageSettings>>();
        
        foreach (var item in options.Value.BucketByExtensions)
        {
            await bucketManager.CreateBucketAsync(item.BucketName);
        }
        return serviceCollection;
    }
}