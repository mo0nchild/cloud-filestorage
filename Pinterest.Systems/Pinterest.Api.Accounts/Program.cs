using Pinterest.Api.Accounts.Configurations;
using Pinterest.Shared.Commons.Middlewares;

namespace Pinterest.Api.Accounts;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddHttpClient();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        await builder.Services.AddAccountsApiServices(builder.Configuration);

        var application = builder.Build();
        if (application.Environment.IsDevelopment())
        {
            application.UseSwagger();
            application.UseSwaggerUI();
        }
        application.UseHttpsRedirection();
        application.UseExceptionsHandler();
        
        application.UseAuthorization();
        application.MapControllers();
        await application.RunAsync();
    }
}