using Pinterest.Domain.Core.Transactions;

namespace Pinterest.MessageBrokers.Saga.Models;

public class SagaOrchestratorSteps<TSagaState, TLockKey> where TSagaState : class
    where TLockKey : IEquatable<TLockKey>
{
    public delegate SagaStep StepFactory(TSagaState state);
    
    public required Func<TSagaState, TLockKey> LockKey { get; set; } 
    public required IReadOnlyList<StepFactory> Steps { get; set; } 
}

public class SagaStep
{
    public required string ServiceName { get; set; }
    public required Func<SagaPayloadBase> StepMessage { get; set; }
}