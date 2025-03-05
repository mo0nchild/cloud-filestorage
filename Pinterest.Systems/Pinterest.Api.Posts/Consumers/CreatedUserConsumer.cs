using Pinterest.Application.Commons.Exceptions;
using Pinterest.Domain.CommonModels.Models;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.Core.Repositories;
using Pinterest.Domain.Messages.UsersMessages;

namespace Pinterest.Api.Posts.Consumers;

public class CreatedUserConsumer : IMessageConsumer<CreatedUserMessage>
{
    private readonly IDocumentRepository<ValidUserInfo> _validUserRepository;

    public CreatedUserConsumer(IDocumentRepository<ValidUserInfo> validUserRepository,
        ILogger<CreatedUserConsumer> logger)
    {
        Logger = logger;
        _validUserRepository = validUserRepository;
    }
    private ILogger<CreatedUserConsumer> Logger { get; }
    
    public async Task ConsumeAsync(CreatedUserMessage message)
    {
        var validUserInfo = new ValidUserInfo() { UserUuid = message.UserUuid };
        
        try { await _validUserRepository.Collection.InsertOneAsync(validUserInfo); }
        catch (ProcessException error)
        {
            Logger.LogWarning($"Failed to adding valid user: {error.Message}");
        }
    }
}