using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.Core.Transactions;
using CloudStorage.MessageBrokers.Saga.Infrastructures;
using CloudStorage.MessageBrokers.Saga.Models;
using CloudStorage.MessageBrokers.Saga.Settings;

namespace CloudStorage.MessageBrokers.Saga.Consumers;

public class ExecuteStepConsumer<TPayload, TRollback> : IMessageConsumer<SagaRequest<TPayload>>
    where TRollback : class, ISagaRollback, new()
    where TPayload : SagaPayloadBase
{
    private readonly ISagaService<TPayload, TRollback> _sagaService;
    private readonly IMessageProducer _producer;
    
    public ExecuteStepConsumer(ISagaService<TPayload, TRollback> sagaService, 
        IMessageProducer producer,
        IOptions<SagaServiceSetting> options,
        ILogger<ExecuteStepConsumer<TPayload, TRollback>> logger)
    {
        _sagaService = sagaService;
        _producer = producer;
        SagaSettings = options.Value;
        Logger = logger;
    }
    private SagaServiceSetting SagaSettings { get; init; }
    private ILogger<ExecuteStepConsumer<TPayload, TRollback>> Logger { get; }
    
    private SagaResponse CreateSagaResponse(SageResponseType responseType, SagaRequest<TPayload> requestMessage, 
        JObject? compensateState = null, 
        string? errorMessage = null, Guid? recordUuid = null) => new()
        {
            SagaUuid = requestMessage.SagaUuid,
            StepName = requestMessage.StepName,
            ResponseType = responseType,
            CompensationState = compensateState,
            ErrorMessage = errorMessage,
            RecordUuid = recordUuid ?? Guid.Empty,
        };
    public async Task ConsumeAsync(SagaRequest<TPayload> message)
    {
        var responsePath = SagaNamingSetting.GetOrchestratorQueueName(SagaSettings.SagaName);
        Logger.LogInformation($"Saga: {message.SagaUuid}, Step: {SagaSettings.ServiceName} - Start execution");
        try
        {
            var compensate = await _sagaService.ExecuteAsync(message.ServiceMessage);
            await _producer.SendAsync(responsePath, CreateSagaResponse(SageResponseType.Completed,
                    requestMessage: message,
                    compensateState: JObject.FromObject(compensate.Rollback),
                    recordUuid: compensate.RecordUuid));
        }
        catch (Exception error)
        {
            Logger.LogError(error.StackTrace);
            Logger.LogWarning($"Saga: {message.SagaUuid}, Step: {SagaSettings.ServiceName} - Reject message: {error.Message}");
            var response = CreateSagaResponse(SageResponseType.Rejected, 
                requestMessage: message,
                errorMessage: error.Message);
            await _producer.SendAsync(responsePath, response);
        }
    }
}