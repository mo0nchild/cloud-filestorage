using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.Core.Transactions;
using CloudStorage.MessageBrokers.Saga.Infrastructures;
using CloudStorage.MessageBrokers.Saga.Models;
using CloudStorage.MessageBrokers.Saga.Settings;

namespace CloudStorage.MessageBrokers.Saga.Consumers;

public class RollbackConsumer<TPayload, TRollback> : IMessageConsumer<SagaRollbackRequest>
    where TRollback : class, ISagaRollback, new()
    where TPayload : SagaPayloadBase
{
    private readonly ISagaService<TPayload, TRollback> _sagaService;
    private readonly IMessageProducer _producer;

    public RollbackConsumer(ISagaService<TPayload, TRollback> sagaService, 
        IMessageProducer producer,
        IOptions<SagaServiceSetting> options,
        ILogger<RollbackConsumer<TPayload, TRollback>> logger)
    {
        _sagaService = sagaService;
        _producer = producer;
        SagaSettings = options.Value;
        Logger = logger;
    }
    private ILogger<RollbackConsumer<TPayload, TRollback>> Logger { get; }
    private SagaServiceSetting SagaSettings { get; init; }
    public async Task ConsumeAsync(SagaRollbackRequest message)
    {
        if (message.RollbackServices.All(item => item != SagaSettings.ServiceName)) return;
        var compensationState = message.CompensateState?.ToObject<TRollback>();
        if (compensationState != null)
        {
            Logger.LogInformation($"Saga: {message.SagaUuid}, Step: {SagaSettings.ServiceName} - Start rollback");
            try { await _sagaService.RollbackAsync(compensationState); }
            catch (Exception error)
            {
                Logger.LogWarning($"Saga: {message.SagaUuid}, Step: {SagaSettings.ServiceName} - Error rollback: {error.Message}");
            }
        }
        else throw new ProcessException("Compensate state is invalid");
    }
}