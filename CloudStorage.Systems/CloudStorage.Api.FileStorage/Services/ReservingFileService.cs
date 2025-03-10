using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Application.FileStorage.Interfaces;
using CloudStorage.Shared.Contracts;
using CloudStorage.Shared.Contracts.FileStoring;
using FileInfo = CloudStorage.Shared.Contracts.FileStoring.FileInfo;
using CloudStorage.Shared.Contracts.Secrets;
using CloudStorage.Shared.Security.Models;
using CloudStorage.Shared.Security.Settings;

namespace CloudStorage.Api.FileStorage.Services;

public class ReservingFileService : FileStoringService.FileStoringServiceBase
{
    private readonly IFileStorageService _fileStorageService;

    public ReservingFileService(IFileStorageService fileStorageService, ILogger<ReservingFileService> logger)
    {
        Logger = logger;
        _fileStorageService = fileStorageService;
    }
    private ILogger<ReservingFileService> Logger { get; }
    
    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    public override async Task<Empty> ReserveFile(FileInfo request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.FileUuid, out var guid))
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
        return new Empty();
    }
}