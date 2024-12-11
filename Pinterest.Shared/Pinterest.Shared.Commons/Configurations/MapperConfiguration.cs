using Microsoft.Extensions.DependencyInjection;
using Pinterest.Shared.Commons.Helpers;

namespace Pinterest.Shared.Commons.Configurations;

public static class MapperConfiguration
{
    public static Task<IServiceCollection> AddModelsMappers(this IServiceCollection collection)
    {
        AutoMappersRegisterHelper.Register(collection);
        return Task.FromResult(collection);
    }
}