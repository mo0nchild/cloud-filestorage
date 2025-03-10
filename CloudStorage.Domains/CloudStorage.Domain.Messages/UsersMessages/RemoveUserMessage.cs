using CloudStorage.Domain.Core.MessageBus;

namespace CloudStorage.Domain.Messages.UsersMessages;

public class RemoveUserMessage : MessageBase
{
    public static readonly string RoutingPath = "remove-user";
    public required Guid UserUuid { get; set; }
}