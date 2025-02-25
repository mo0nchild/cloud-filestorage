using Pinterest.Domain.Core.MessageBus;

namespace Pinterest.Domain.Messages.UsersMessages;

public class CreatedUserMessage : MessageBase
{
    public static readonly string RoutingPath = "user-created";
    public required Guid UserUuid { get; set; }
}