using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CloudStorage.Application.FileStorage.Infrastructures.Interfaces;
using CloudStorage.S3Storage.VideoProcessing.Settings;

namespace CloudStorage.S3Storage.VideoProcessing;

public static class Bootstrapper
{
    private static readonly string ProcessingSettingsSection = "VideoProcessing";
    public static Task<IServiceCollection> AddVideoProcessingServices(this IServiceCollection serviceCollection, 
        IConfiguration configuration)
    {
        serviceCollection.Configure<ProcessingSettings>(configuration.GetSection(ProcessingSettingsSection));
        serviceCollection.AddTransient<IVideoProcessing, Services.VideoProcessing>();
        return Task.FromResult(serviceCollection);
    }
}