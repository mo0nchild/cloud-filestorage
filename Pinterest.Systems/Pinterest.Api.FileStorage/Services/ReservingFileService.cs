using Grpc.Core;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.FileStorage.Interfaces;
using Pinterest.Shared.Contracts;
using Pinterest.Shared.Contracts.FileStoring;
using FileInfo = Pinterest.Shared.Contracts.FileStoring.FileInfo;
using Pinterest.Shared.Contracts.Secrets;

namespace Pinterest.Api.FileStorage.Services;

public class ReservingFileService : FileStoringService.FileStoringServiceBase
{
    private readonly IFileStorageService _fileStorageService;

    public ReservingFileService(IFileStorageService fileStorageService, ILogger<ReservingFileService> logger)
    {
        Logger = logger;
        _fileStorageService = fileStorageService;
    }
    private ILogger<ReservingFileService> Logger { get; }

    public override async Task<EmptyResult> ReserveFile(FileInfo request, ServerCallContext context)
    {
        if (Guid.TryParse(request.FileUuid, out var guid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid GUID"));
        }
        try {
            await _fileStorageService.SetFileIsUsing(guid);
            Logger.LogError($"Reserve file {request.FileUuid} successfully");
        }
        catch (ProcessException error)
        {
            Logger.LogError($"Failing reserve file {request.FileUuid}: {error.Message}");
            throw new RpcException(new Status(StatusCode.NotFound, error.Message));
        }
        return new EmptyResult();
    }
}