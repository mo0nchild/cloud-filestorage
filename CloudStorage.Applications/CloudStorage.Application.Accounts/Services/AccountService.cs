using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CloudStorage.Application.Accounts.Interfaces;
using CloudStorage.Application.Accounts.Models;
using CloudStorage.Application.Accounts.Repositories;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Application.Tokens.Interfaces;
using CloudStorage.Domain.Authorization.Entities;
using CloudStorage.Domain.Core.Factories;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.Messages.AccountMessages;
using CloudStorage.Shared.Commons.Validations;

namespace CloudStorage.Application.Accounts.Services;

using BCryptType = BCrypt.Net.BCrypt;
internal class AccountService : IAccountsService
{
    private readonly RepositoryFactoryInterface<IAccountsRepository> _contextFactory;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IMessageProducer _messageProducer;
    private readonly IModelValidator<CredentialsModel> _credentialsValidator;

    public AccountService(RepositoryFactoryInterface<IAccountsRepository> contextFactory, 
        IMapper mapper, ITokenService tokenService,
        IMessageProducer messageProducer,
        IModelValidator<CredentialsModel> credentialsValidator)
    {
        _contextFactory = contextFactory;
        _mapper = mapper;
        _tokenService = tokenService;
        _messageProducer = messageProducer;
        _credentialsValidator = credentialsValidator;
    }
    protected virtual Claim[] GenerateClaims(AccountModel model) => new Claim[]
    {
        new Claim(ClaimTypes.PrimarySid, model.UserUuid.ToString()),
        new Claim(ClaimTypes.Email, model.Email),
        new Claim(ClaimTypes.Role, model.Role.ToString())
    };
    
    public async Task<IdentityModel> GetTokensByCredentials(CredentialsModel credentials)
    {
        var verifyPassword = (string hashPassword) =>
        {
            try { return BCryptType.Verify(credentials.Password, hashPassword); }
            catch (BCrypt.Net.SaltParseException) { return false; }
        };
        using var dbContext = await _contextFactory.CreateRepositoryAsync();
        var profilesList = await dbContext.AccountInfos
            .Where(item => item.Email == credentials.Email).ToListAsync();

        var profile = profilesList.FirstOrDefault(item => 
            item.Email == credentials.Email && verifyPassword(item.Password));
        ProcessException.ThrowIf(() => profile == null, $"Account {credentials.Email} does not exist");

        var profileClaims = GenerateClaims(_mapper.Map<AccountModel>(profile));
        var tokens = await _tokenService.CreateJwtTokens(profileClaims);
        profile!.RefreshToken = tokens.RefreshToken;
        await dbContext.SaveChangesAsync();
        
        return _mapper.Map<IdentityModel>(tokens);
    }
    public async Task<IdentityModel> GetTokensByRefreshToken(string refreshToken)
    {
        var userClaims = await _tokenService.VerifyRefreshToken(refreshToken);
        var userUuid = userClaims?.FirstOrDefault(item => item.Type == ClaimTypes.PrimarySid);

        ProcessException.ThrowIf(() => userClaims == null || userUuid == null, "Error in token validation");
        using var dbContext = await _contextFactory.CreateRepositoryAsync();
        
        var profile = await dbContext.AccountInfos.FirstOrDefaultAsync(item => item.RefreshToken == refreshToken);

        if (profile == null) throw new ProcessException("Account is not found");
        var profileClaims = GenerateClaims(_mapper.Map<AccountModel>(profile));
        var tokens = await _tokenService.CreateJwtTokens(profileClaims);

        var identityInstance = _mapper.Map<IdentityModel>(tokens);
        profile.RefreshToken = identityInstance.RefreshToken;

        await dbContext.SaveChangesAsync();
        return identityInstance;
    }
    public async Task<AccountModel?> GetAccountByAccessToken(string accessToken)
    {
        var userClaims = await _tokenService.VerifyAccessToken(accessToken);
        var userUuid = userClaims?.FirstOrDefault(item => item.Type == ClaimTypes.PrimarySid);

        AuthException.ThrowIf(() => userClaims == null || userUuid == null, "Token is not valid");
        using var dbContext = await _contextFactory.CreateRepositoryAsync();
        
        var userProfile = await dbContext.AccountInfos
            .FirstOrDefaultAsync(item => item.UserUuid == Guid.Parse(userUuid!.Value));
        if (userProfile == null)
        {
            throw new ProcessException($"Account does not exist");
        }
        return _mapper.Map<AccountModel>(userProfile);
    }
    public async Task<Guid> CreateAccount(Guid userUuid, CredentialsModel credentials)
    {
        await _credentialsValidator.CheckAsync(credentials);
        
        var userRecord = _mapper.Map<AccountInfo>(credentials);
        userRecord.UserUuid = userUuid;

        using var dbContext = await _contextFactory.CreateRepositoryAsync();
        await dbContext.AccountInfos.AddRangeAsync(userRecord);
        
        await dbContext.SaveChangesAsync();
        return userRecord.Uuid;
    }
    public async Task DeleteAccount(string accessToken)
    {
        var userClaims = await _tokenService.VerifyAccessToken(accessToken);
        var userEmail = userClaims?.FirstOrDefault(item => item.Type == ClaimTypes.Email);

        AuthException.ThrowIf(() => userClaims == null || userEmail == null, "Token is not valid");
        
        using var dbContext = await _contextFactory.CreateRepositoryAsync();
        var account = await dbContext.AccountInfos.FirstOrDefaultAsync(item => item.Email == userEmail!.Value);
        
        ProcessException.ThrowIf(() => account == null, "Account does not exist");
        dbContext.AccountInfos.RemoveRange(account!);
        await dbContext.SaveChangesAsync();
        
        await _messageProducer.SendToAllAsync(RemoveAccountMessage.RoutingPath, new RemoveAccountMessage()
        {
            UserUuid = account!.UserUuid
        });
    }
}