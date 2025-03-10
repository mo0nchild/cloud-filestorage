using CloudStorage.Domain.Core.MessageBus;

namespace CloudStorage.Domain.Messages.FileStorageMessages;

public class FileReserveMessage : MessageBase
{
    public static readonly string RoutingPath = "file-reserve";
    public required Guid FileUuid { get; set; }
}