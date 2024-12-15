using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Models;
using Pinterest.Api.Posts.Configurations;
using Pinterest.Shared.Commons.Middlewares;
using Pinterest.Shared.Security.Configurations;
using Pinterest.Shared.Security.Swagger;

namespace Pinterest.Api.Posts;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddHttpClient();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Pinterest.Api.Posts", Version = "v1" });
            options.OperationFilter<AuthorizationHeaderFilter>();
        });
        builder.Services.Configure<FormOptions>(options => {
            options.ValueCountLimit = int.MaxValue;
        });
        await builder.Services.AddAccountsApiServices(builder.Configuration);
        await builder.Services.AddIdentityServices(builder.Configuration);

        var application = builder.Build();
        if (application.Environment.IsDevelopment())
        {
            application.UseSwagger();
            application.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pinterest.Api.Posts");
            });
        }
        application.UseHttpsRedirection();
        application.UseExceptionsHandler();
        
        application.UseAuthentication();
        application.UseAuthorization();
        application.MapControllers();
        await application.RunAsync();
    }
}