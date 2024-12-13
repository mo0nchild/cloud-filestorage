using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.Tokens.Interfaces;
using Pinterest.Application.Tokens.Models;
using Pinterest.Domain.Authorization.Settings;

namespace Pinterest.Application.Tokens.Services;

public class TokenService(IOptions<TokenOptions> tokenOptions, ILogger<TokenService> logger)
    : ITokenService
{
    private readonly TokenOptions _tokenOptions = tokenOptions.Value;
    protected ILogger<TokenService> Logger { get; } = logger;

    protected virtual string GenerateToken(Claim[] claims, byte[] symmetricKey, int expires)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(symmetricKey);
        var securityToken = tokenHandler.CreateToken(new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expires),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        });
        return tokenHandler.WriteToken(securityToken);
    }
    public Task<TokensModel> CreateJwtTokens(Claim[] claims)
    {
        var accessSymmetricKey = Encoding.UTF8.GetBytes(_tokenOptions.AccessSecret);
        /*var refreshSymmetricKey = Encoding.UTF8.GetBytes(_tokenOptions.RefreshSecret);*/
            
        return Task.FromResult(new TokensModel()
        {
            AccessToken = GenerateToken(claims, accessSymmetricKey, _tokenOptions.AccessExpires),
        });
    }
    protected virtual Claim[]? ValidateToken(string token, byte[] symmetricKey)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(symmetricKey),
            ValidateIssuer = false, 
            ValidateAudience = false,
        };
        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            if (jwtToken.ValidTo < DateTime.UtcNow) throw new ProcessException("Token expired");
            return principal.Claims.ToArray();
        }
        catch (Exception error)
        {
            Logger.LogWarning($"Error in token validation: {error.Message}");
            return null;
        }
    } 
    public Task<Claim[]?> VerifyAccessToken(string token)
    {
        return Task.FromResult(ValidateToken(token, Encoding.UTF8.GetBytes(_tokenOptions.AccessSecret)));   
    }

    public Task<Claim[]?> VerifyRefreshToken(string token)
    {
        throw new NotImplementedException();
    }
}