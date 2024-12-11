using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.Messages;
using Pinterest.MessageBrokers.RabbitMQ;

namespace TestBroker;

static class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        await using (var scope = host.Services.CreateAsyncScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var producer = services.GetRequiredService<IMessageProducer>();
                await producer.SendAsync<TestMessage>("test", new()
                {
                    Name = "Hello from TestBroker"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        await host.RunAsync();
    }
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                
                services.AddProducerService("test", hostContext.Configuration);
            });
}