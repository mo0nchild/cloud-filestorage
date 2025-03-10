using CloudStorage.Application.Accounts.Models;
using CloudStorage.Application.Accounts.Sagas;
using CloudStorage.Domain.Core.Transactions;

namespace CloudStorage.Application.Accounts.Interfaces;

public interface IRegistrateAccount
{
    Task<Guid> RegistrateAccountAsync(RegistrationModel registrationModel);
    Task<AccountSagaInfo> GetRegistrateAccountStateAsync(Guid sagaId);
}