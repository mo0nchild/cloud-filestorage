using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Application.FileStorage.Interfaces;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.Messages.FileStorageMessages;

namespace CloudStorage.Api.FileStorage.Consumers;

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