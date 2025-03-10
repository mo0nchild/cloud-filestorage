using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.GrpcServices.Core.Settings;
using CloudStorage.Shared.Contracts;
using FileInfo = System.IO.FileInfo;

namespace CloudStorage.GrpcServices.Core.Utils;

public class GrpcServiceHelper
{
    private static readonly ILogger<GrpcServiceHelper> Logger = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
        builder.SetMinimumLevel(LogLevel.Debug);
    }).CreateLogger<GrpcServiceHelper>();
    
    public static async Task<TResponse> SendRequestToService<TService, TResponse>(GrpcSettingsBase serviceSettings, 
        Func<GrpcChannel, TService> serviceFactory,
        Func<TService, Task<TResponse>> handler) where TService : ClientBase
    {
        if (serviceSettings.ServicePath == string.Empty)
        {
            Logger.LogError("Grpc service path is empty");
            throw new Exception("Grpc service path is empty");
        }
        using var channel = GrpcChannel.ForAddress(serviceSettings.ServicePath);
        
        try { return await handler.Invoke(serviceFactory.Invoke(channel)); }
        catch (RpcException error) when (error.StatusCode == StatusCode.Unavailable)
        {
            Logger.LogError("Unable to access to GRPC service");
            throw new ProcessException(error.Message);
        }
        catch (RpcException error)
        {
            Logger.LogError($"Process exception: {error.StatusCode} - {error.Message}");
            throw new ProcessException(error.Message);
        }
    } 
}