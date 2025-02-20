using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pinterest.Application.Users.Infrastructures;
using Pinterest.GrpcServices.Core.Utils;
using Pinterest.GrpcServices.Users.Settings;
using Pinterest.Shared.Contracts;
using Pinterest.Shared.Contracts.FileStoring;
using FileInfo = Pinterest.Shared.Contracts.FileStoring.FileInfo;
using FileStoringServiceClient = Pinterest.Shared.Contracts.FileStoring.FileStoringService.FileStoringServiceClient;

namespace Pinterest.GrpcServices.Users.Services;

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