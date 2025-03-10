using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Application.Users.Interfaces;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.Messages.AccountMessages;

namespace CloudStorage.Api.Users.Consumers;

public class RemoveAccountConsumer : IMessageConsumer<RemoveAccountMessage>
{
    private readonly IUserService _userService;
    public RemoveAccountConsumer(IUserService userService, ILogger<RemoveAccountConsumer> logger)
    {
        Logger = logger;
        _userService = userService;
    }
    private ILogger<RemoveAccountConsumer> Logger { get; }
    
    public async Task ConsumeAsync(RemoveAccountMessage message)
    {
        try { await _userService.DeleteUserAsync(message.UserUuid); }
        catch (ProcessException error)
        {
            Logger.LogWarning($"Failed to delete user: {error.Message}");
        }
    }
}