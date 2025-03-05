using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.Users.Interfaces;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.Messages.AccountMessages;

namespace Pinterest.Api.Users.Consumers;

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