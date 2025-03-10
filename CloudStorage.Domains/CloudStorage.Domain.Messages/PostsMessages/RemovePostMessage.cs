using CloudStorage.Domain.Core.MessageBus;

namespace CloudStorage.Domain.Messages.PostsMessages;

public class RemovePostMessage : MessageBase
{
    public static readonly string RoutingPath = "remove-post";
    public required Guid PostUuid { get; set; }
}