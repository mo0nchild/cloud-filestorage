using AutoMapper;
using Pinterest.Application.Accounts.Interfaces;
using Pinterest.Application.Accounts.Models;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Domain.Core.Transactions;
using Pinterest.Domain.Messages.SagaMessages.CreateAccountSaga;

namespace Pinterest.Application.Accounts.Services;

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