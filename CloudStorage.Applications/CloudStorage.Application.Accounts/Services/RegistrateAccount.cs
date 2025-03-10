using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CloudStorage.Application.Accounts.Interfaces;
using CloudStorage.Application.Accounts.Models;
using CloudStorage.Application.Accounts.Sagas;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Domain.Core.Transactions;
using CloudStorage.Shared.Commons.Validations;

namespace CloudStorage.Application.Accounts.Services;

internal class RegistrateAccount : IRegistrateAccount
{
    private readonly ISagaManager<AccountSagaState, Guid> _sagaManager;
    private readonly IMapper _mapper;
    private readonly IModelValidator<RegistrationModel> _registrationModelValidator;

    public RegistrateAccount([FromKeyedServices("RegistrateAccount")] ISagaManager<AccountSagaState, Guid> sagaManager, 
        IMapper mapper,
        IModelValidator<RegistrationModel> registrationModelValidator,
        ILogger<RegistrateAccount> logger)
    {
        _sagaManager = sagaManager;
        _mapper = mapper;
        _registrationModelValidator = registrationModelValidator;
        Logger = logger;
    }
    public ILogger<RegistrateAccount> Logger { get; }
    
    public async Task<Guid> RegistrateAccountAsync(RegistrationModel registrationModel)
    {
        await _registrationModelValidator.CheckAsync(registrationModel);
        return await _sagaManager.StartSaga(_mapper.Map<AccountSagaState>(registrationModel));
    }
    public async Task<AccountSagaInfo> GetRegistrateAccountStateAsync(Guid stateUuid)
    {
        var sagaInfo = await _sagaManager.GetSagaInfo(stateUuid) ?? 
                       throw new ProcessException("Registrate account not found");
        return _mapper.Map<AccountSagaInfo>(sagaInfo);
    }
}