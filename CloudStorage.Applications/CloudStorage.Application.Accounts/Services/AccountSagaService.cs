using AutoMapper;
using CloudStorage.Application.Accounts.Interfaces;
using CloudStorage.Application.Accounts.Models;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Domain.Core.Transactions;
using CloudStorage.Domain.Messages.SagaMessages.CreateAccountSaga;

namespace CloudStorage.Application.Accounts.Services;

public class AccountSagaService : ISagaService<CreateAccountPayload, CreateAccountCompensation>
{
    private readonly IAccountsService _accountsService;
    private readonly IMapper _mapper;

    public AccountSagaService(IAccountsService accountsService, IMapper mapper)
    {
        _accountsService = accountsService;
        _mapper = mapper;
    }
    public async Task<(Guid RecordUuid, CreateAccountCompensation Rollback)> ExecuteAsync(
        SagaServiceMessage<CreateAccountPayload> request)
    {
        if (!request.ServicesRecords.TryGetValue(CreateUserServiceInfo.ServiceName, out var userUuid))
        {
            throw new ProcessException("Cannot find user service info");
        }
        var recordUuid = await _accountsService.CreateAccount(userUuid,
            credentials: _mapper.Map<CredentialsModel>(request.Payload));
        return (recordUuid, new CreateAccountCompensation() { Email = request.Payload.Email });
    }
    public async Task RollbackAsync(CreateAccountCompensation rollback)
    {
        await _accountsService.DeleteAccount(rollback.Email);
    }
}