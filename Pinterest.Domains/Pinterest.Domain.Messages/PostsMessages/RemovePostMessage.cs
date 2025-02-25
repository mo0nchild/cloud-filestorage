using Pinterest.Domain.Core.MessageBus;

namespace Pinterest.Domain.Messages.PostsMessages;

public class RemovePostMessage : MessageBase
{
    public static readonly string RoutingPath = "remove-post";
    public required Guid PostUuid { get; set; }
}