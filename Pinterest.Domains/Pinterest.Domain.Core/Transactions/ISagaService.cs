namespace Pinterest.Domain.Core.Transactions;

public interface ISagaService<TPayload, TRollback> where TPayload : SagaPayloadBase
    where TRollback : ISagaRollback
{
    Task<(Guid RecordUuid, TRollback Rollback)> ExecuteAsync(SagaServiceMessage<TPayload> request);
    Task RollbackAsync(TRollback rollback);
}