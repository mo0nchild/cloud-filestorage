using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CloudStorage.Shared.Commons.Helpers;

public static class ValidatorsRegisterHelper 
{
    public static Task Register(IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(s => s.FullName != null && s.FullName.ToLower().StartsWith("CloudStorage."));

        assemblies.ToList().ForEach(x => { services.AddValidatorsFromAssembly(x, ServiceLifetime.Singleton); });
        return Task.CompletedTask;
    }
}