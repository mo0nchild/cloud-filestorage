using Pinterest.Application.Accounts;
using Pinterest.Application.Tokens;
using Pinterest.Database.Accounts;
using Pinterest.Shared.Commons.Configurations;

namespace Pinterest.Api.Accounts.Configurations;

public static class ApiServicesConfigurations
{
    public static async Task<IServiceCollection> AddAccountsApiServices(this IServiceCollection collection,
        IConfiguration configuration)
    {
        await collection.AddModelsMappers();
        await collection.AddModelsValidators();

        await collection.AddAccountServices();
        await collection.AddTokensServices(configuration);
        await collection.AddAccountsDatabase(configuration);
        return collection;
    }
}