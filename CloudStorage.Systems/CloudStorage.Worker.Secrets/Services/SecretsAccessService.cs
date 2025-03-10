using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using CloudStorage.Domain.Authorization.Settings;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.Messages.TokenSecretsMessages;
using TokenOptions = CloudStorage.Domain.Authorization.Settings.TokenOptions;

namespace CloudStorage.Worker.SecretsStorage.Services;

public class SecretsAccessService(IOptions<TokenOptions> tokenOptions, IMessageProducer producer, 
    ILogger<SecretsAccessService> logger)
{
    private readonly IMessageProducer _producer = producer;
    private readonly TokenOptions _tokenOptions = tokenOptions.Value;
    private readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TokenOptions, NewSecretsMessage>();
        })
        .CreateMapper();
    
    private ILogger<SecretsAccessService> Logger { get; } = logger;
    public async Task SendNewSecretsNotification()
    {
        await _producer.SendToAllAsync(NewSecretsMessage.RoutingPath, _mapper.Map<NewSecretsMessage>(_tokenOptions));
    }
    public Task<TokenOptions> GetTokenSecrets() => Task.FromResult(_tokenOptions);
}