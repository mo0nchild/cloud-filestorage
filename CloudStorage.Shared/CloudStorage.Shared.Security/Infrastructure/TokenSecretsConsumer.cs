using AutoMapper;
using Microsoft.Extensions.Logging;
using CloudStorage.Domain.Authorization.Settings;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.Messages.TokenSecretsMessages;

namespace CloudStorage.Shared.Security.Infrastructure;

internal class TokenSecretsConsumer(TokenSecretsSettings tokenSecretsSettings,
    ILogger<TokenSecretsConsumer> logger) : IMessageConsumer<NewSecretsMessage>
{
    private readonly TokenSecretsSettings _tokenSecretsSettings = tokenSecretsSettings;
    private ILogger<TokenSecretsConsumer> Logger { get; } = logger;
    public Task ConsumeAsync(NewSecretsMessage message)
    {
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<NewSecretsMessage, TokenOptions>())
            .CreateMapper();
        _tokenSecretsSettings.Secrets = mapper.Map<TokenOptions>(message);
        
        Logger.LogInformation($"New secret message received");
        return Task.CompletedTask;
    }
}