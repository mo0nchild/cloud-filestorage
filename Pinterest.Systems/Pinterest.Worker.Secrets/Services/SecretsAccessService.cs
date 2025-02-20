using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Pinterest.Domain.Authorization.Settings;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.Messages.TokenSecretsMessages;
using TokenOptions = Pinterest.Domain.Authorization.Settings.TokenOptions;

namespace Pinterest.Worker.SecretsStorage.Services;

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