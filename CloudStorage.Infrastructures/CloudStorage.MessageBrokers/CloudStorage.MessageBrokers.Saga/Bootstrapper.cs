using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CloudStorage.Documents.Mongo;
using CloudStorage.Documents.Mongo;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.Core.Repositories;
using CloudStorage.Domain.Core.Transactions;
using CloudStorage.MessageBrokers.RabbitMQ;
using CloudStorage.MessageBrokers.RabbitMQ.Settings;
using CloudStorage.MessageBrokers.Saga.Consumers;
using CloudStorage.MessageBrokers.Saga.Infrastructures;
using CloudStorage.MessageBrokers.Saga.Models;
using CloudStorage.MessageBrokers.Saga.Settings;

namespace CloudStorage.MessageBrokers.Saga;

public static class Bootstrapper
{
    public static async Task<IServiceCollection> AddSagaOrchestrator<TSagaState, TLockKey>(this IServiceCollection serviceCollection,
        SagaOrchestratorSetting orchestratorSetting,
        SagaOrchestratorSteps<TSagaState, TLockKey> orchestratorSteps,
        IConfiguration configuration) 
    where TLockKey : IEquatable<TLockKey>
    where TSagaState : class
    {
        await serviceCollection.AddMongoDbServices<SagaStoreEntity<TSagaState, TLockKey>>($"saga-{orchestratorSetting.SagaName}");
        serviceCollection.AddKeyedSingleton<SagaOrchestrator<TSagaState, TLockKey>>(orchestratorSetting.SagaName, (provider, _) =>
        {
            var repository = provider.GetRequiredService<IDocumentRepository<SagaStoreEntity<TSagaState, TLockKey>>>();
            var producer = provider.GetRequiredService<IMessageProducer>();
            
            return new SagaOrchestrator<TSagaState, TLockKey>(orchestratorSteps,
                options: new OptionsWrapper<SagaOrchestratorSetting>(orchestratorSetting),
                producer: producer,
                repository: repository,
                logger: provider.GetRequiredService<ILogger<SagaOrchestrator<TSagaState, TLockKey>>>());
        });
        await MessageConsumerRegistator.Registrate<SagaOrchestratorConsumer<TSagaState, TLockKey>, SagaResponse>(
            collection: serviceCollection,
            consumerTag: $"saga-{orchestratorSetting.SagaName}",
            consumerFactory: provider => new SagaOrchestratorConsumer<TSagaState, TLockKey>(
                orchestrator: provider.GetRequiredKeyedService<SagaOrchestrator<TSagaState, TLockKey>>(orchestratorSetting.SagaName), 
                logger: provider.GetRequiredService<ILogger<SagaOrchestratorConsumer<TSagaState, TLockKey>>>()));
        
        await serviceCollection.AddConsumerListener<SagaResponse>(new RoutingOptions()
        {
            ConsumerTag = $"saga-{orchestratorSetting.SagaName}",
            QueueName = SagaNamingSetting.GetOrchestratorQueueName(orchestratorSetting.SagaName),
        }, configuration);

        serviceCollection.AddKeyedTransient<ISagaManager<TSagaState, TLockKey>, SagaManager<TSagaState, TLockKey>>(orchestratorSetting.SagaName,
            (provider, _) =>
            {
                var orchestrator = provider.GetRequiredKeyedService<SagaOrchestrator<TSagaState, TLockKey>>(orchestratorSetting.SagaName);
                return new SagaManager<TSagaState, TLockKey>(orchestrator);
            });
        return serviceCollection;
    }
    
    private static string GetExecuteTag(string sagaName, string serviceName) => $"{sagaName}-{serviceName}-execute";
    private static string GetRollbackTag(string sagaName, string serviceName) => $"{sagaName}-{serviceName}-rollback";
    public static async Task<IServiceCollection> AddSagaService<TSagaService, TPayload, TRollback>(this IServiceCollection serviceCollection,
        SagaServiceSetting serviceSetting, IConfiguration configuration)
        where TSagaService : class, ISagaService<TPayload, TRollback>
        where TRollback : class, ISagaRollback, new()
        where TPayload : SagaPayloadBase
    {
        serviceCollection.AddKeyedSingleton<ISagaService<TPayload, TRollback>, TSagaService>(serviceSetting.SagaName);

        await MessageConsumerRegistator.Registrate<ExecuteStepConsumer<TPayload, TRollback>, SagaRequest<TPayload>>(
            collection: serviceCollection,
            consumerTag: GetExecuteTag(serviceSetting.SagaName, serviceSetting.ServiceName),
            consumerFactory: provider =>
            {
                var sagaService = provider.GetRequiredKeyedService<ISagaService<TPayload, TRollback>>(serviceSetting.SagaName);
                var producer = provider.GetRequiredService<IMessageProducer>();
                
                return new ExecuteStepConsumer<TPayload, TRollback>(sagaService, producer, 
                    options: new OptionsWrapper<SagaServiceSetting>(serviceSetting), 
                    logger: provider.GetRequiredService<ILogger<ExecuteStepConsumer<TPayload, TRollback>>>());
            });
        await MessageConsumerRegistator.Registrate<RollbackConsumer<TPayload, TRollback>, SagaRollbackRequest>(
            collection: serviceCollection,
            consumerTag: GetRollbackTag(serviceSetting.SagaName, serviceSetting.ServiceName),
            consumerFactory: provider =>
            {
                var sagaService = provider.GetRequiredKeyedService<ISagaService<TPayload, TRollback>>(serviceSetting.SagaName);
                var producer = provider.GetRequiredService<IMessageProducer>();
                
                return new RollbackConsumer<TPayload, TRollback>(sagaService, producer, 
                    options: new OptionsWrapper<SagaServiceSetting>(serviceSetting),
                    logger: provider.GetRequiredService<ILogger<RollbackConsumer<TPayload, TRollback>>>());
            });
        await serviceCollection.AddConsumerListener<SagaRequest<TPayload>>(new RoutingOptions()
        {
            ConsumerTag = GetExecuteTag(serviceSetting.SagaName, serviceSetting.ServiceName),
            QueueName = SagaNamingSetting.GetExecuteQueueName(serviceSetting.SagaName, serviceSetting.ServiceName),
        }, configuration);
        await serviceCollection.AddConsumerListener<SagaRollbackRequest>(new RoutingOptions()
        {
            ConsumerTag = GetRollbackTag(serviceSetting.SagaName, serviceSetting.ServiceName),
            QueueName = SagaNamingSetting.GetRollbackQueueName(serviceSetting.SagaName, serviceSetting.ServiceName),
        }, configuration);
        return serviceCollection;
    }
}