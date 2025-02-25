using Pinterest.Api.FileStorage.Configurations;
using Pinterest.Api.FileStorage.Services;
using Pinterest.Shared.Commons;
using Pinterest.Shared.Commons.Middlewares;
using Pinterest.Shared.Security;

namespace Pinterest.Api.FileStorage;

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