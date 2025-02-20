using Pinterest.Shared.Commons;
using Pinterest.Shared.Commons.Configurations;
using Pinterest.Worker.SecretsStorage.Configurations;
using Pinterest.Worker.SecretsStorage.Services;

namespace Pinterest.Worker.SecretsStorage;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddGrpc();
        builder.Services.AddHealthChecks();
        await builder.Services.AddSecretsStorageServices(builder.Configuration);
        await builder.Services.AddCoreConfiguration(builder.Configuration);

        var application = builder.Build();
        await using (var scope = application.Services.CreateAsyncScope())
        {
            var secretsService = scope.ServiceProvider.GetRequiredService<SecretsAccessService>();
            await secretsService.SendNewSecretsNotification();
        }
        application.UseCoreConfiguration();
        application.UseHealthChecks("/health");
        application.MapGrpcService<SecretsServiceImpl>();
        await application.RunAsync();
    }
}