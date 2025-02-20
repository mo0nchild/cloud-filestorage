using Pinterest.Domain.Core.MessageBus;

namespace Pinterest.Domain.Messages.AccountMessages;

public class RemoveAccountMessage : MessageBase
{
    public static readonly string RoutingPath = "remove-account";
    public required Guid UserUuid { get; set; }
}