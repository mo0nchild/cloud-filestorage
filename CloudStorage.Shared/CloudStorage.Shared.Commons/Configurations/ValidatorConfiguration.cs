using Microsoft.Extensions.DependencyInjection;
using CloudStorage.Shared.Commons.Helpers;
using CloudStorage.Shared.Commons.Validations;

namespace CloudStorage.Shared.Commons.Configurations;

public static class ValidatorConfiguration
{
    internal static Task<IServiceCollection> AddModelsValidators(this IServiceCollection collection)
    {
        //collection.AddFluentValidationAutoValidation(options =>
        //{
        //    options.DisableDataAnnotationsValidation = false,
        //});
        ValidatorsRegisterHelper.Register(collection);
        collection.AddTransient(typeof(IModelValidator<>), typeof(ModelValidator<>));

        return Task.FromResult(collection);
    }
}