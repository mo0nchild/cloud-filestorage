using Pinterest.Domain.Core.MessageBus;

namespace Pinterest.Domain.Messages.FileStorageMessages;

public class FileReserveMessage : MessageBase
{
    public static readonly string RoutingPath = "file-reserve";
    public required Guid FileUuid { get; set; }
}