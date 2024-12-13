using Pinterest.Application.Accounts.Models;
using Pinterest.Application.Accounts.Services;
using Pinterest.Domain.Authorization.Entities;

namespace Pinterest.Application.Accounts.Interfaces;

public interface IAccountsService
{
    public Task<IdentityModel> GetTokensByCredentials(CredentialsModel credentials);
    public Task<AccountModel?> GetAccountByAccessToken(string accessToken);
    
    public Task<IdentityModel> Registration(RegistrationModel registrationModel);
}