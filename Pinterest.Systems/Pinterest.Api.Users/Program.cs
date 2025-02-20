using Pinterest.Api.Users.Configurations;
using Pinterest.Shared.Commons;
using Pinterest.Shared.Commons.Configurations;
using Pinterest.Shared.Commons.Middlewares;
using Pinterest.Shared.Security;
using Pinterest.Shared.Security.Configurations;

namespace Pinterest.Api.Users;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHealthChecks();

        await builder.Services.AddSecurityServices(builder.Configuration);
        await builder.Services.AddApiServices(builder.Configuration);
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
        application.MapControllers();
        await application.RunAsync();
    }
}