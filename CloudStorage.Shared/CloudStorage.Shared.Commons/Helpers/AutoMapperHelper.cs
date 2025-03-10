using Microsoft.Extensions.DependencyInjection;

namespace CloudStorage.Shared.Commons.Helpers;

public static class AutoMappersRegisterHelper
{
    public static Task Register(IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(s => s.FullName != null && s.FullName.ToLower().StartsWith("CloudStorage."));

        services.AddAutoMapper(assemblies);
        return Task.CompletedTask;
    }
}