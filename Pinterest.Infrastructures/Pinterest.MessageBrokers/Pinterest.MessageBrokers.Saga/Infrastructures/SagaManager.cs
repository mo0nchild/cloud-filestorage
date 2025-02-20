using Pinterest.Domain.Core.Transactions;
using Pinterest.MessageBrokers.Saga.Models;

namespace Pinterest.MessageBrokers.Saga.Infrastructures;

public class SagaManager<TSagaState, TLockKey> : ISagaManager<TSagaState, TLockKey> where TSagaState : class
    where TLockKey : IEquatable<TLockKey>
{
    private readonly SagaOrchestrator<TSagaState, TLockKey> _sagaOrchestrator;
    public SagaManager(SagaOrchestrator<TSagaState, TLockKey> sagaOrchestrator)
    {
        _sagaOrchestrator = sagaOrchestrator;
    }
    public async Task<Guid> StartSaga(TSagaState state) => await _sagaOrchestrator.StartSagaAsync(state);

    public async Task<SagaStoreEntity<TSagaState, TLockKey>?> GetSagaInfo(Guid sagaUuid)
    {
        return await _sagaOrchestrator.GetSagaState(sagaUuid);
    }
}