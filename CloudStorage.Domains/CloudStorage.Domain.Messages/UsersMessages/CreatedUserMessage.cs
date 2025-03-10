using CloudStorage.Domain.Core.MessageBus;

namespace CloudStorage.Domain.Messages.UsersMessages;

public class CreatedUserMessage : MessageBase
{
    public static readonly string RoutingPath = "user-created";
    public required Guid UserUuid { get; set; }
}