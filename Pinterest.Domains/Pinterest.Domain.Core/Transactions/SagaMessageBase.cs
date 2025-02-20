namespace Pinterest.Domain.Core.Transactions;

public abstract class SagaPayloadBase { }

public interface ISagaRollback { }

public class SagaServiceMessage<TPayload> where TPayload : SagaPayloadBase
{
    public required TPayload Payload { get; set; }
    public required IReadOnlyDictionary<string, Guid> ServicesRecords { get; set; }
}