using CloudStorage.Api.Users.Configurations;
using CloudStorage.Api.Users.Services;
using CloudStorage.Shared.Commons;
using CloudStorage.Shared.Commons.Configurations;
using CloudStorage.Shared.Commons.Middlewares;
using CloudStorage.Shared.Security;
using CloudStorage.Shared.Security.Configurations;

namespace CloudStorage.Api.Users;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddGrpc().AddJsonTranscoding();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHealthChecks();

        await builder.Services.AddSecurityServices(builder.Configuration);
        await builder.Services.AddUsersApiServices(builder.Configuration);
        await builder.Services.AddCoreConfiguration(builder.Configuration);
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
        application.MapGrpcService<AuthorSubscribersServiceImpl>();
        application.MapControllers();
        await application.RunAsync();
    }
}