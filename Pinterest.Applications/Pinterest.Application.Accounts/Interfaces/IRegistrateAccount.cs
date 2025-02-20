using Pinterest.Application.Accounts.Models;
using Pinterest.Application.Accounts.Sagas;
using Pinterest.Domain.Core.Transactions;

namespace Pinterest.Application.Accounts.Interfaces;

public interface IRegistrateAccount
{
    Task<Guid> RegistrateAccountAsync(RegistrationModel registrationModel);
    Task<AccountSagaInfo> GetRegistrateAccountStateAsync(Guid sagaId);
}