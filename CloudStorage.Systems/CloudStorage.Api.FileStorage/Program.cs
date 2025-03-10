using CloudStorage.Api.FileStorage.Configurations;
using CloudStorage.Api.FileStorage.Services;
using CloudStorage.Shared.Commons;
using CloudStorage.Shared.Commons.Middlewares;
using CloudStorage.Shared.Security;

namespace CloudStorage.Api.FileStorage;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
 
        builder.Services.AddHealthChecks();
        builder.Services.AddGrpc().AddJsonTranscoding();
        builder.Services.AddHostedService<CleaningHostedService>();
        
        await builder.Services.AddSecurityServices(builder.Configuration);
        await builder.Services.AddCoreConfiguration(builder.Configuration);
        await builder.Services.AddApiServices(builder.Configuration);
        await builder.Services.AddSecretService(builder.Configuration);

        var application = builder.Build();
        if (application.Environment.IsDevelopment())
        {
            application.UseSwagger();
            application.UseSwaggerUI();
        }
        application.UseHttpsRedirection();
        application.UseCoreConfiguration();
        application.UseSecurity();
        
        application.UseHealthChecks("/health");
        application.MapGrpcService<ReservingFileService>();
        application.MapControllers();

        await application.RunAsync();
    }
}