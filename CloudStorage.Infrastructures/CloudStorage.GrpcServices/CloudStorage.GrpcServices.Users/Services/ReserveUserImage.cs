using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CloudStorage.Application.Users.Infrastructures;
using CloudStorage.GrpcServices.Core.Utils;
using CloudStorage.GrpcServices.Users.Settings;
using CloudStorage.Shared.Contracts;
using CloudStorage.Shared.Contracts.FileStoring;
using FileInfo = CloudStorage.Shared.Contracts.FileStoring.FileInfo;
using FileStoringServiceClient = CloudStorage.Shared.Contracts.FileStoring.FileStoringService.FileStoringServiceClient;

namespace CloudStorage.GrpcServices.Users.Services;

internal class ReserveUserImage : IReserveUserImage
{
    public ReserveUserImage(IOptions<ReserveImageSettings> options, ILogger<ReserveUserImage> logger)
    {
        Settings = options.Value;
        Logger = logger;
    }
    private ReserveImageSettings Settings { get; }
    private ILogger<ReserveUserImage> Logger { get; }
    
    public async Task ReserveUserImageAsync(Guid imageUuid)
    {
        await GrpcServiceHelper.SendRequestToService(serviceFactory: channel => new FileStoringServiceClient(channel),
            serviceSettings: Settings,
            handler: async client => await client.ReserveFileAsync(new FileInfo() { FileUuid = imageUuid.ToString() }));
    }
}