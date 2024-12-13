using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pinterest.Application.Accounts.Interfaces;
using Pinterest.Application.Accounts.Models;
using Pinterest.Application.Accounts.Repositories;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.Tokens.Interfaces;
using Pinterest.Domain.Authorization.Entities;
using Pinterest.Domain.Core.Factories;
using Pinterest.Shared.Commons.Validations;

namespace Pinterest.Application.Accounts.Services;

using BCryptType = BCrypt.Net.BCrypt;
public class AccountService : IAccountsService
{
    private readonly RepositoryFactoryInterface<IAccountsRepository> _contextFactory;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IModelValidator<RegistrationModel> _registrationValidator;

    public AccountService(RepositoryFactoryInterface<IAccountsRepository> contextFactory, 
        IMapper mapper, ITokenService tokenService, 
        IModelValidator<RegistrationModel> registrationValidator)
    {
        _contextFactory = contextFactory;
        _mapper = mapper;
        _tokenService = tokenService;
        _registrationValidator = registrationValidator;
    }
    protected virtual Claim[] GenerateClaims(AccountModel model) => new Claim[]
    {
        new Claim(ClaimTypes.PrimarySid, model.Uuid.ToString()),
        new Claim(ClaimTypes.Email, model.Email),
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

        var identityInstance = _mapper.Map<IdentityModel>(tokens);
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
            .FirstOrDefaultAsync(item => item.Uuid == Guid.Parse(userUuid!.Value));
        if (userProfile == null)
        {
            throw new ProcessException($"Account does not exist");
        }
        return _mapper.Map<AccountModel>(userProfile);
    }
    public async Task<IdentityModel> Registration(RegistrationModel registrationModel)
    {
        var userRecord = _mapper.Map<AccountInfo>(registrationModel);
        await _registrationValidator.CheckAsync(registrationModel);

        using var dbContext = await _contextFactory.CreateRepositoryAsync();
        var profileClaims = GenerateClaims(_mapper.Map<AccountModel>(userRecord));

        var tokens = await _tokenService.CreateJwtTokens(profileClaims);

        await dbContext.AccountInfos.AddRangeAsync(userRecord);
        await dbContext.SaveChangesAsync();
        
        return _mapper.Map<IdentityModel>(tokens);
    }
}