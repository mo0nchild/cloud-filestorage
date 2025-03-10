using CloudStorage.Application.Accounts;
using CloudStorage.Application.Accounts.Sagas;
using CloudStorage.Application.Accounts.Services;
using CloudStorage.Application.Tokens;
using CloudStorage.Database.Accounts;
using CloudStorage.Documents.Mongo;
using CloudStorage.Domain.Messages.SagaMessages.CreateAccountSaga;
using CloudStorage.MessageBrokers.RabbitMQ;
using CloudStorage.MessageBrokers.Saga;
using CloudStorage.MessageBrokers.Saga.Models;
using CloudStorage.MessageBrokers.Saga.Settings;

namespace CloudStorage.Api.Accounts.Configurations;

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