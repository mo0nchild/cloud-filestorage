using CloudStorage.Domain.Core.MessageBus;

namespace CloudStorage.Domain.Messages.AccountMessages;

public class RemoveAccountMessage : MessageBase
{
    public static readonly string RoutingPath = "remove-account";
    public required Guid UserUuid { get; set; }
}