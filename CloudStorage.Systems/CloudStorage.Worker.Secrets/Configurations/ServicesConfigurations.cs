using CloudStorage.Domain.Authorization.Settings;
using CloudStorage.MessageBrokers.RabbitMQ;
using CloudStorage.Worker.SecretsStorage.Services;

namespace CloudStorage.Worker.SecretsStorage.Configurations;

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