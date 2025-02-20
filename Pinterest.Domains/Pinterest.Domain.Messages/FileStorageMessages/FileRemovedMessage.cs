using Pinterest.Domain.Core.MessageBus;

namespace Pinterest.Domain.Messages.FileStorageMessages;

public class FileRemovedMessage : MessageBase
{
    public static readonly string RoutingPath = "file-removed";
    public required Guid FileUuid { get; set; }
}