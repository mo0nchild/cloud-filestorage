using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CloudStorage.Application.Users.Infrastructures;
using CloudStorage.GrpcServices.Core.Configurations;
using CloudStorage.GrpcServices.Users.Services;
using CloudStorage.GrpcServices.Users.Settings;

namespace CloudStorage.GrpcServices.Users;

public static class Bootstrapper
{
    public static async Task<IServiceCollection> AddUsersGrpcServices(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        await serviceCollection.AddGrpcServiceSetting<ReserveImageSettings>(configuration);
        
        serviceCollection.AddTransient<IReserveUserImage, ReserveUserImage>();
        return serviceCollection;
    }
}