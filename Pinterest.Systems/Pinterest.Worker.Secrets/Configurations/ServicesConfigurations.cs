using Pinterest.Domain.Authorization.Settings;
using Pinterest.MessageBrokers.RabbitMQ;
using Pinterest.Worker.SecretsStorage.Services;

namespace Pinterest.Worker.SecretsStorage.Configurations;

public static class ServicesConfigurations
{
    private static readonly string TokenSection = "Tokens";
    public static async Task<IServiceCollection> AddSecretsStorageServices(this IServiceCollection serviceCollection, 
        IConfiguration configuration)
    {
        var tokenConfiguration = new ConfigurationBuilder()
            .AddJsonFile("tokenSecrets.json")
            .Build();
        serviceCollection.Configure<TokenOptions>(tokenConfiguration.GetSection(TokenSection));
        serviceCollection.AddScoped<SecretsAccessService>();
        await serviceCollection.AddProducerService(configuration);
        return serviceCollection;
    }
}