using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pinterest.Application.Users.Infrastructures;
using Pinterest.GrpcServices.Core.Configurations;
using Pinterest.GrpcServices.Users.Services;
using Pinterest.GrpcServices.Users.Settings;

namespace Pinterest.GrpcServices.Users;

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