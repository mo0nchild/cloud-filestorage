using System.Collections.Immutable;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.Core.Repositories;
using Pinterest.Domain.Core.Transactions;
using Pinterest.MessageBrokers.Saga.Models;
using Pinterest.MessageBrokers.Saga.Settings;

namespace Pinterest.MessageBrokers.Saga.Infrastructures;

public class SagaOrchestrator<TSagaState, TLockKey> where TSagaState : class
    where TLockKey : IEquatable<TLockKey>
{
    private readonly SagaOrchestratorSteps<TSagaState, TLockKey> _orchestratorSteps;
    private readonly IMessageProducer _producer;
    private readonly IDocumentRepository<SagaStoreEntity<TSagaState, TLockKey>> _repository;

    public SagaOrchestrator(SagaOrchestratorSteps<TSagaState, TLockKey> orchestratorSteps,
        ILogger<SagaOrchestrator<TSagaState, TLockKey>> logger,
        IOptions<SagaOrchestratorSetting> options,
        IMessageProducer producer,
        IDocumentRepository<SagaStoreEntity<TSagaState, TLockKey>> repository)
    {
        _orchestratorSteps = orchestratorSteps;
        _producer = producer;
        _repository = repository;
        
        (Logger, SagaSettings) = (logger, options.Value);
        InitializeAsync().Wait();
    }
    public record StepInfo(Guid SagaUuid, string ResponseStep, string? ResponseMessage, Guid RecordUuid);
    private ILogger<SagaOrchestrator<TSagaState, TLockKey>> Logger { get; init; }
    private SagaOrchestratorSetting SagaSettings { get; init; }

    private async Task InitializeAsync()
    {
        var updater = _repository.UpdateBuilder.Set(item => item.Status, SagaStatus.Cancelled);
        await _repository.Collection.UpdateManyAsync(item => item.Status == SagaStatus.Running, updater);
        
        var aggregation = await _repository.Collection.Aggregate()
            .Match(item => item.Status == SagaStatus.Pending)
            .Group(x => x.LockKey, x => new { Status = x.Key, Sagas = x.ToList() })
            .ToListAsync();
        foreach (var item in aggregation)
        {
            var pending = item.Sagas.FirstOrDefault();
            if (pending is not null) await RunReservedSaga(pending.LockKey);
        }
    }
    private async Task<bool> CheckIfRunningExistsAsync(TLockKey lockKey)
    {
        var saga = await _repository.Collection
            .Find(item => item.LockKey.Equals(lockKey) && item.Status == SagaStatus.Running)
            .FirstOrDefaultAsync();
        return saga is not null;
    }
    private async Task UpdateStep<TProp>(StepInfo step, Expression<Func<SagaStepEntity, TProp>> property, TProp value)
    {
        var filterQuery =_repository.RepositoryFilter.Eq(item => item.SagaUuid, step.SagaUuid) 
                         & _repository.RepositoryFilter.ElemMatch(prop => prop.SagaSteps, item => item.ServiceName == step.ResponseStep);
        
        var updateQuery = _repository.UpdateBuilder.Set($"SagaSteps.$.{GetPropertyPath(property)}", value);
        await _repository.Collection.UpdateOneAsync(filterQuery, updateQuery);
        
        string GetPropertyPath(Expression<Func<SagaStepEntity, TProp>> selector)
        {
            if (selector.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            throw new ArgumentException("Invalid property expression");
        }
    }
    private async Task RunReservedSaga(TLockKey lockKey)
    {
        var saga = _repository.Collection.Find(item => item.LockKey.Equals(lockKey) && item.Status == SagaStatus.Pending)
            .FirstOrDefault();
        if (saga == null) return;
        var sagaStep = _orchestratorSteps.Steps.Select(item => item(saga.SagaState)).First();
        
        var servicePath = SagaNamingSetting.GetExecuteQueueName(SagaSettings.SagaName, sagaStep.ServiceName);
        await _producer.SendAsync(servicePath, new SagaRequest<SagaPayloadBase>()
        {
            StepName = sagaStep.ServiceName,
            SagaUuid = saga.SagaUuid,
            ServiceMessage = new SagaServiceMessage<SagaPayloadBase>()
            {
                Payload = sagaStep.StepMessage(),
                ServicesRecords = saga.ServicesRecords
            },
        });
    }
    public async Task<Guid> StartSagaAsync(TSagaState state)
    {
        if (_orchestratorSteps.Steps.Count <= 0) throw new Exception("Saga step count is zero");
        var lockKeyUsed = await CheckIfRunningExistsAsync(_orchestratorSteps.LockKey(state));
        var storeState = new SagaStoreEntity<TSagaState, TLockKey>()
        {
            LockKey = _orchestratorSteps.LockKey(state),
            SagaState = state,
            Status = lockKeyUsed ? SagaStatus.Pending : SagaStatus.Running,
            SagaSteps = _orchestratorSteps.Steps.Select(step => new SagaStepEntity()
            {
                ServiceName = step(state).ServiceName,
            }).ToImmutableList(),   
        };
        await _repository.Collection.InsertOneAsync(storeState);
        if (!lockKeyUsed)
        {
            var firstStep = _orchestratorSteps.Steps[0](state);
            var servicePath = SagaNamingSetting.GetExecuteQueueName(SagaSettings.SagaName, firstStep.ServiceName);
        
            await _producer.SendAsync(servicePath, new SagaRequest<SagaPayloadBase>()
            {
                StepName = firstStep.ServiceName,
                SagaUuid = storeState.SagaUuid,
                ServiceMessage = new SagaServiceMessage<SagaPayloadBase>()
                {
                    Payload = firstStep.StepMessage(),
                    ServicesRecords = storeState.ServicesRecords
                }
            });
        }
        return storeState.SagaUuid;
    }
    public async Task<SagaStoreEntity<TSagaState, TLockKey>?> GetSagaState(Guid sagaUuid)
    {
        var filter = Builders<SagaStoreEntity<TSagaState, TLockKey>>.Filter.Eq(item => item.SagaUuid, sagaUuid);
        return await _repository.Collection.Find(filter).FirstOrDefaultAsync();
    }
    public async Task NextStepAsync(StepInfo responseStep, JObject? compensation)
    {
        await UpdateStep(responseStep, it => it.Status, value: SagaStepStatus.Completed);
        if (compensation != null)
        {
            await UpdateStep(responseStep, it => it.CompensationData, value: BsonDocument.Parse(compensation.ToString()));
        }
        Logger.LogInformation($"Saga - {responseStep.SagaUuid}, Step completed - {responseStep.ResponseStep}");
        var saga = await _repository.Collection.Find(item => item.SagaUuid == responseStep.SagaUuid).FirstAsync();
        var servicesRecords = saga.ServicesRecords.Concat(new[]
        {
            new KeyValuePair<string, Guid>(responseStep.ResponseStep, responseStep.RecordUuid)
        }).ToDictionary();
        var nextStep = saga.SagaSteps.FirstOrDefault(item => item.Status == SagaStepStatus.Processing); 
        if (nextStep != null)
        {
            var servicePath = SagaNamingSetting.GetExecuteQueueName(SagaSettings.SagaName, nextStep.ServiceName);
            var stepActions = _orchestratorSteps.Steps.Select(it => it(saga.SagaState))
                .First(it => it.ServiceName == nextStep.ServiceName);
            
            await _producer.SendAsync(servicePath, new SagaRequest<SagaPayloadBase>()
            {
                StepName = nextStep.ServiceName,
                SagaUuid = saga.SagaUuid,
                ServiceMessage = new SagaServiceMessage<SagaPayloadBase>()
                {
                    Payload = stepActions.StepMessage(),
                    ServicesRecords = servicesRecords
                },
            });
            return;
        }
        var updater = _repository.UpdateBuilder.Combine(new []
        {
            _repository.UpdateBuilder.Set(item => item.Status, SagaStatus.Completed),
            _repository.UpdateBuilder.Set(item => item.ServicesRecords, servicesRecords)
        });
        await _repository.Collection.UpdateOneAsync(item => item.SagaUuid == saga.SagaUuid, updater);
        Logger.LogInformation($"Saga - {saga.SagaUuid}, Saga was completed");
        await RunReservedSaga(saga.LockKey);
    }
    public async Task RollbackSagaAsync(StepInfo responseStep)
    {
        await UpdateStep(responseStep, it => it.Status, value: SagaStepStatus.Rejected);
        var saga = await _repository.Collection.Find(item => item.SagaUuid == responseStep.SagaUuid).FirstAsync();
        
        Logger.LogInformation($"Saga - {responseStep.SagaUuid}, Step rejected - {responseStep.ResponseStep}");
        var rollbackSteps = saga.SagaSteps.Where(it => it is { Status: SagaStepStatus.Completed, CompensationData: not null })
            .ToImmutableList();
        foreach (var rollbackStep in rollbackSteps)
        {
            var servicePath = SagaNamingSetting.GetRollbackQueueName(SagaSettings.SagaName, rollbackStep.ServiceName);
            var stepActions = _orchestratorSteps.Steps.Select(it => it(saga.SagaState))
                .First(it => it.ServiceName == rollbackStep.ServiceName);
            await _producer.SendAsync(servicePath, new SagaRollbackRequest()
            {
                RollbackServices = rollbackSteps.Select(item => item.ServiceName).ToImmutableList(),
                SagaUuid = saga.SagaUuid,
                CompensateState = JObject.Parse(rollbackStep.CompensationData.ToJson()),
            });
        }
        var updater = _repository.UpdateBuilder.Combine(new []
        {
            _repository.UpdateBuilder.Set(item => item.Status, SagaStatus.Rollback),
            _repository.UpdateBuilder.Set(item => item.RollbackMessage, new RollbackSagaMessage()
            {
                ErrorMessage = responseStep.ResponseMessage ?? string.Empty,
            })
        });
        await _repository.Collection.UpdateOneAsync(item => item.SagaUuid == saga.SagaUuid, updater);
        await RunReservedSaga(saga.LockKey);

    }
    /*public async Task CancelSagaStep(StepInfo responseStep)
    {
        await UpdateStep(responseStep, it => it.Status, value: SagaStepStatus.Cancelled);
        var saga = await _repository.Collection.Find(item => item.SagaUuid == responseStep.SagaUuid).FirstAsync();
        if (saga.SagaSteps.All(it => it.Status != SagaStepStatus.Completed))
        {
            var updater = _repository.UpdateBuilder.Set(item => item.Status, SagaStatus.Completed);
            await _repository.Collection.UpdateOneAsync(item => item.SagaUuid == saga.SagaUuid, updater);
            await RunReservedSaga(saga.LockKey);
        }
    }*/
}