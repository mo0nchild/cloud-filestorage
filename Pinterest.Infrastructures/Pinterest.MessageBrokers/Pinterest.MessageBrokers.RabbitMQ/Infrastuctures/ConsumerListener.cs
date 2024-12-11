using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.MessageBrokers.RabbitMQ.Implementations;
using Pinterest.MessageBrokers.RabbitMQ.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Pinterest.MessageBrokers.RabbitMQ.Infrastuctures;

public class ConsumerListener<TMessage> : BackgroundService, IAsyncDisposable
    where TMessage : MessageBase
{
    private readonly IMessageConsumer<TMessage> _consumer;
    private readonly string _queueName;
    private readonly BrokerBaseSetting _brokerSetting;

    private IConnection? _connection;
    private IChannel? _channel;
    public ILogger<ConsumerListener<TMessage>> Logger { get; private set; }
    public ConsumerListener(string queueName, IMessageConsumer<TMessage> consumer, 
        IOptions<BrokerBaseSetting> brokerSetting,
        ILogger<ConsumerListener<TMessage>> logger)
    {
        _queueName = queueName;
        _consumer = consumer;
        _brokerSetting = brokerSetting.Value;
        Logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConsumeMessage();
        while (!stoppingToken.IsCancellationRequested) await Task.Delay(100, stoppingToken);
    }
    public async Task InitializeAsync()
    {
        var factory = new ConnectionFactory()
        {
            Uri = new Uri(_brokerSetting.Uri),
            UserName = _brokerSetting.UserName,
            Password = _brokerSetting.Password,

            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
        };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.QueueDeclareAsync(queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }
    public virtual async ValueTask DisposeAsync()
    {
        if (_connection != null) await _connection.DisposeAsync();
        if (_channel != null) await _channel.DisposeAsync();
    }
    protected virtual async Task ConsumeMessage()
    {
        if (_channel == null) throw new NullReferenceException("Channel is null");
          
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, @event) =>
        {
            var message = Encoding.UTF8.GetString(@event.Body.ToArray());
            var messageObject = JsonConvert.DeserializeObject<TMessage>(message);
            if (messageObject != null)
            {
                await _consumer.ConsumeAsync(messageObject);
            }
        };
        await _channel.BasicConsumeAsync(queue: _queueName,
            autoAck: true,
            consumer: consumer);
    }
}