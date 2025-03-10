using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CloudStorage.Application.Tokens;
using CloudStorage.Domain.Authorization.Settings;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.Messages.TokenSecretsMessages;
using CloudStorage.MessageBrokers.RabbitMQ;
using CloudStorage.MessageBrokers.RabbitMQ.Settings;
using CloudStorage.Shared.Contracts.Secrets;
using CloudStorage.Shared.Security.Infrastructure;
using CloudStorage.Shared.Security.Settings;

namespace CloudStorage.Shared.Security.Configurations;

public static class SecretsConfiguration
{
    private static readonly string TokenSection = "Tokens";
    private static readonly string SecretsSection = "Secrets";
    internal static async Task<IServiceCollection> AddSecretsConfiguration(this IServiceCollection servicesCollection, 
            IConfiguration configuration)
    {
        await servicesCollection.AddTokensServices(configuration);
        
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        var secretOptions = configuration.GetSection(SecretsSection).Get<SecretsApiOptions>();
        
        var tokenSecrets = await GetTokenSecrets(factory.CreateLogger("Program"), secretOptions?.ApiRoute);
        servicesCollection.AddSingleton<TokenSecretsSettings>(_ => new TokenSecretsSettings()
        {
            Secrets = tokenSecrets
        });
        servicesCollection.AddSingleton<IMessageConsumer<NewSecretsMessage>, TokenSecretsConsumer>();
        await servicesCollection.AddConsumerListener<NewSecretsMessage>(new RoutingOptions()
        {
            QueueName = Guid.NewGuid().ToString(),
            ExchangeName = NewSecretsMessage.RoutingPath
        }, configuration);
        return servicesCollection;
    }
    private static async Task<TokenOptions> GetTokenSecrets(ILogger logger, string? secretsApiRoute)
    {
        if (secretsApiRoute != null)
        {
            using var channel = GrpcChannel.ForAddress(secretsApiRoute);
            var secretsClient = new SecretsService.SecretsServiceClient(channel);
            try
            {
                var secretsResult = await secretsClient.GetSecretsAsync(new Empty());
                if (secretsResult != null)
                {
                    var mapper = new MapperConfiguration(cfg => cfg.CreateMap<SecretsInfo, TokenOptions>())
                        .CreateMapper();
                    return mapper.Map<TokenOptions>(secretsResult);
                }
            }
            catch (RpcException error) when (error.StatusCode == StatusCode.Unavailable)
            {
                logger.LogWarning("Unable to retrieve token secrets from Secret Service");
            }
        }
        var tokenConfiguration = new ConfigurationBuilder()
            .AddJsonFile("tokenSecrets.json")
            .Build();
        var defaultSecrets = tokenConfiguration.GetSection(TokenSection).Get<TokenOptions>();
        if (defaultSecrets == null) throw new Exception($"Token Secret configuration file is missing or invalid");
        return defaultSecrets;
    }
}