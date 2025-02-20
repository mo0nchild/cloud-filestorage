using Microsoft.Extensions.Logging;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.MessageBrokers.Saga.Infrastructures;
using Pinterest.MessageBrokers.Saga.Models;

namespace Pinterest.MessageBrokers.Saga.Consumers;

public class SagaOrchestratorConsumer<TSagaState, TLockKey> : IMessageConsumer<SagaResponse>
    where TSagaState : class
    where TLockKey : IEquatable<TLockKey>
{
    private readonly SagaOrchestrator<TSagaState, TLockKey> _orchestrator;
    public SagaOrchestratorConsumer(SagaOrchestrator<TSagaState, TLockKey> orchestrator, 
        ILogger<SagaOrchestratorConsumer<TSagaState, TLockKey>> logger)
    {
        _orchestrator = orchestrator;
        Logger = logger;
    }
    private ILogger<SagaOrchestratorConsumer<TSagaState, TLockKey>> Logger { get; init; }
    public async Task ConsumeAsync(SagaResponse message)
    {
        var sagaStep = new SagaOrchestrator<TSagaState, TLockKey>.StepInfo(message.SagaUuid, 
            ResponseStep: message.StepName, 
            ResponseMessage: message.ErrorMessage, 
            RecordUuid: message.RecordUuid);
        switch (message.ResponseType)
        {
            case SageResponseType.Completed:
                Logger.LogInformation($"Saga: {sagaStep.SagaUuid}, Step: {sagaStep.ResponseStep} - Completed");
                await _orchestrator.NextStepAsync(sagaStep, message.CompensationState);
                break;
            case SageResponseType.Rejected:
                Logger.LogInformation($"Saga: {sagaStep.SagaUuid}, Step: {sagaStep.ResponseStep} - Rejected");
                await _orchestrator.RollbackSagaAsync(sagaStep);
                break;
            /*case SageResponseType.Cancelled:
                await _orchestrator.CancelSagaStep(sagaStep);
                break;*/
        }
    }
}