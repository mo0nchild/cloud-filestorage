using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.FileStorage.Interfaces;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.Messages.FileStorageMessages;

namespace Pinterest.Api.FileStorage.Consumers;

public class DeleteFileConsumer : IMessageConsumer<FileRemovedMessage>
{
    private readonly IFileStorageService _fileStorageService;

    public DeleteFileConsumer(IFileStorageService fileStorageService, ILogger<DeleteFileConsumer> logger)
    {
        Logger = logger;
        _fileStorageService = fileStorageService;
    }
    private ILogger<DeleteFileConsumer> Logger { get; }

    public async Task ConsumeAsync(FileRemovedMessage message)
    {
        try {
            await _fileStorageService.DeleteFile(message.FileUuid);
            Logger.LogInformation($"Deleted file {message.FileUuid}");
        }
        catch (ProcessException error)
        {
            Logger.LogError($"Consumer failing to delete file: {error.Message}");
        }
    }
}