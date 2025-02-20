namespace Pinterest.Domain.Core.Transactions;

public interface ISagaManager<TSagaState, TLockKey> where TSagaState : class
    where TLockKey : IEquatable<TLockKey>
{
    Task<Guid> StartSaga(TSagaState state);
    Task<SagaStoreEntity<TSagaState, TLockKey>?> GetSagaInfo(Guid sagaUuid);
}