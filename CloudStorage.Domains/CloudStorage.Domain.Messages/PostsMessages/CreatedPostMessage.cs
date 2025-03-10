using CloudStorage.Domain.Core.MessageBus;

namespace CloudStorage.Domain.Messages.PostsMessages;

public class CreatedPostMessage : MessageBase
{
    public static readonly string RoutingPath = "post-created";
    public required Guid PostUuid { get; set; }
}