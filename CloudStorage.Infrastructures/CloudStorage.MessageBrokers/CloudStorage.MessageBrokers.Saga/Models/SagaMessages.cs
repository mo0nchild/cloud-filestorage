using Newtonsoft.Json.Linq;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.Core.Transactions;

namespace CloudStorage.MessageBrokers.Saga.Models;

public class SagaRequest<TPayload> : MessageBase where TPayload : SagaPayloadBase
{
    public required Guid SagaUuid { get; set; }
    public required string StepName { get; set; }
    public required SagaServiceMessage<TPayload> ServiceMessage { get; set; }
}
public enum SageResponseType
{
    Completed,
    Rejected,
    Cancelled,
}
public class SagaResponse : MessageBase
{
    public required Guid SagaUuid { get; set; }
    public required string StepName { get; set; }
    public required SageResponseType ResponseType { get; set; }
    public JObject? CompensationState { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid RecordUuid { get; set; }
}

public class SagaRollbackRequest : MessageBase
{
    public required Guid SagaUuid { get; set; }
    public IReadOnlyList<string> RollbackServices { get; set; } = new List<string>();
    public JObject? CompensateState { get; set; }
    public int RetryCount { get; set; }
}