using Microsoft.Extensions.DependencyInjection;
using CloudStorage.Shared.Commons.Helpers;

namespace CloudStorage.Shared.Commons.Configurations;

public static class MapperConfiguration
{
    internal static Task<IServiceCollection> AddModelsMappers(this IServiceCollection collection)
    {
        AutoMappersRegisterHelper.Register(collection);
        return Task.FromResult(collection);
    }
}