using MongoDB.Bson;
using Pinterest.Domain.Core.Models;

namespace Pinterest.Domain.Core.Transactions;

public enum SagaStatus { Pending, Running, Rollback, Completed, Cancelled }
public class SagaStoreEntity<TSagaState, TLockKey> : BaseEntity 
    where TSagaState : class
{
    public Guid SagaUuid { get; set; } = Guid.NewGuid();
    public required TSagaState SagaState { get; set; }
    public required TLockKey LockKey { get; set; }
    public SagaStatus Status { get; set; } = SagaStatus.Pending;
    public RollbackSagaMessage? RollbackMessage { get; set; }
    public IReadOnlyDictionary<string, Guid> ServicesRecords { get; set; } = new Dictionary<string, Guid>();
    public required IReadOnlyList<SagaStepEntity>  SagaSteps { get; set; }
}
public enum SagaStepStatus
{
    Completed,
    Processing,
    Rejected,
    Cancelled,
}
public class SagaStepEntity
{
    public required string ServiceName { get; set; }
    public SagaStepStatus Status { get; set; } = SagaStepStatus.Processing;
    public BsonDocument? CompensationData { get; set; }
}

public class RollbackSagaMessage
{
    public required string ErrorMessage { get; set; }
}