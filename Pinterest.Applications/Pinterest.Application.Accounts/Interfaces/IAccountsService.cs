using Pinterest.Application.Accounts.Models;
using Pinterest.Application.Accounts.Services;
using Pinterest.Domain.Authorization.Entities;

namespace Pinterest.Application.Accounts.Interfaces;

public interface IAccountsService
{
    public Task<IdentityModel> GetTokensByCredentials(CredentialsModel credentials);
    public Task<IdentityModel> GetTokensByRefreshToken(string refreshToken);
    public Task<AccountModel?> GetAccountByAccessToken(string accessToken);
    
    public Task<Guid> CreateAccount(Guid userUuid, CredentialsModel credentials);
    public Task DeleteAccount(string accessToken);
}