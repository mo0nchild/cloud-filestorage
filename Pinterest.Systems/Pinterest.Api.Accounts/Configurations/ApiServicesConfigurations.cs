using Pinterest.Application.Accounts;
using Pinterest.Application.Accounts.Sagas;
using Pinterest.Application.Accounts.Services;
using Pinterest.Application.Tokens;
using Pinterest.Database.Accounts;
using Pinterest.Documents.Mongo;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.Messages.AccountMessages;
using Pinterest.Domain.Messages.SagaMessages.CreateAccountSaga;
using Pinterest.MessageBrokers.RabbitMQ;
using Pinterest.MessageBrokers.RabbitMQ.Settings;
using Pinterest.MessageBrokers.Saga;
using Pinterest.MessageBrokers.Saga.Models;
using Pinterest.MessageBrokers.Saga.Settings;
using Pinterest.Shared.Commons.Configurations;
using Pinterest.Shared.Security.Configurations;

namespace Pinterest.Api.Accounts.Configurations;

public static class ApiServicesConfigurations
{
    private static readonly string RegistrateAccountSagaName = "RegistrateAccount";
    public static async Task<IServiceCollection> AddAccountsApiServices(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        await serviceCollection.AddTokensServices(configuration);
        
        await serviceCollection.AddAccountServices();
        await serviceCollection.AddAccountsDatabase(configuration);
     
        await serviceCollection.AddMongoClient(configuration);
        await serviceCollection.AddProducerService(configuration);
        await serviceCollection.AddSagaOrchestrator(
            orchestratorSetting: new SagaOrchestratorSetting() { SagaName = RegistrateAccountSagaName },
            configuration: configuration,
            orchestratorSteps: new SagaOrchestratorSteps<AccountSagaState, Guid>()
            {
                Steps = new List<SagaOrchestratorSteps<AccountSagaState, Guid>.StepFactory>()
                {
                    state => new SagaStep()
                    {
                        ServiceName = CreateUserServiceInfo.ServiceName,
                        StepMessage = () => new CreateUserPayload()
                        {
                            Email = state.Email,
                            Username = state.Username,
                        }
                    },
                    state => new SagaStep()
                    {
                        ServiceName = CreateAccountServiceInfo.ServiceName,
                        StepMessage = () => new CreateAccountPayload()
                        {
                            Email = state.Email,
                            Password = state.Password,
                        }
                    }
                },
                LockKey = state => state.Uuid
            });
        await serviceCollection.AddSagaService<AccountSagaService, CreateAccountPayload, CreateAccountCompensation>(
            new SagaServiceSetting()
            {
                SagaName = RegistrateAccountSagaName,
                ServiceName = CreateAccountServiceInfo.ServiceName,
            },
            configuration);
        
        return serviceCollection;
    }
}