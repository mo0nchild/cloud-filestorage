using CloudStorage.Api.Accounts.Configurations;
using CloudStorage.Shared.Commons;
using CloudStorage.Shared.Commons.Configurations;
using CloudStorage.Shared.Commons.Middlewares;
using CloudStorage.Shared.Security;

namespace CloudStorage.Api.Accounts;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddHttpClient();
        
        builder.Services.AddHealthChecks();
        builder.Services.AddEndpointsApiExplorer();
        
        await builder.Services.AddCoreConfiguration(builder.Configuration);
        await builder.Services.AddAccountsApiServices(builder.Configuration);
        await builder.Services.AddSecretService(builder.Configuration);

        var application = builder.Build();
        if (application.Environment.IsDevelopment())
        {
            application.UseSwagger();
            application.UseSwaggerUI();
        }
        application.UseHttpsRedirection();
        application.UseCoreConfiguration();
        
        application.UseHealthChecks("/health");
        application.MapControllers();
        await application.RunAsync();
    }
}