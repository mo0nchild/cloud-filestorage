using Microsoft.Extensions.DependencyInjection;
using Pinterest.Shared.Commons.Helpers;
using Pinterest.Shared.Commons.Validations;

namespace Pinterest.Shared.Commons.Configurations;

public static class ValidatorConfiguration
{
    public static Task<IServiceCollection> AddModelsValidators(this IServiceCollection collection)
    {
        //collection.AddFluentValidationAutoValidation(options =>
        //{
        //    options.DisableDataAnnotationsValidation = false,
        //});
        ValidatorsRegisterHelper.Register(collection);
        collection.AddScoped(typeof(IModelValidator<>), typeof(ModelValidator<>));

        return Task.FromResult(collection);
    }
}