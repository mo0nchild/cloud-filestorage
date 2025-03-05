using Pinterest.Application.Commons.Exceptions;
using Pinterest.Domain.CommonModels.Models;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.Core.Repositories;
using Pinterest.Domain.Messages.UsersMessages;

namespace Pinterest.Api.Posts.Consumers;

public class RemovedUserConsumer : IMessageConsumer<RemoveUserMessage>
{
    private readonly IDocumentRepository<ValidUserInfo> _validUserRepository;

    public RemovedUserConsumer(IDocumentRepository<ValidUserInfo> validUserRepository,
        ILogger<RemovedUserConsumer> logger)
    {
        Logger = logger;
        _validUserRepository = validUserRepository;
    }
    private ILogger<RemovedUserConsumer> Logger { get; }
    
    public async Task ConsumeAsync(RemoveUserMessage message)
    {
        var filter = _validUserRepository.RepositoryFilter.Eq(item => item.UserUuid, message.UserUuid);
        
        try { await _validUserRepository.Collection.DeleteOneAsync(filter); }
        catch (ProcessException error)
        {
            Logger.LogWarning($"Failed to removed valid user: {error.Message}");
        }
    }
}