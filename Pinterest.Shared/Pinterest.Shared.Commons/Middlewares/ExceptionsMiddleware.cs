using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pinterest.Application.Commons.Exceptions;

namespace Pinterest.Shared.Commons.Middlewares;

public record class ExceptionMessage(string Title, string Errors);
public class ExceptionsMiddleware(RequestDelegate next, ILogger<ExceptionsMiddleware> logger)
{
    protected ILogger<ExceptionsMiddleware> Logger { get;  } = logger;

    public async Task Invoke(HttpContext context)
    {
        try { await next(context); }
        catch (ProcessException error)
        {
            Logger.LogWarning($"An exception occurred during the request: {error.GetType().Name}");
            Logger.LogWarning($"Exception message: {error.Message}");
            
            await HandleExceptionAsync(context, error);
        }
    }
    protected virtual async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var result = new ExceptionMessage(
            Title: $"Exception type: {exception.GetType().Name}",
            Errors: exception.Message
        );  
        await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
    }
}
public static class ExceptionsMiddlewareExtension
{
    public static IApplicationBuilder UseExceptionsHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionsMiddleware>();
    }
}