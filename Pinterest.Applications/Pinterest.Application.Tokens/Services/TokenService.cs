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

internal class TokenService : ITokenService
{
    private TokenOptions _tokenOptions;
    public TokenService(TokenSecretsSettings secretsSettings, ILogger<TokenService> logger)
    {
        _tokenOptions = secretsSettings.Secrets;
        Logger = logger;
        secretsSettings.OnSecretsUpdated(newSecrets =>
        {
            _tokenOptions = newSecrets;
        });
    }
    private ILogger<TokenService> Logger { get; }
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
        var refreshSymmetricKey = Encoding.UTF8.GetBytes(_tokenOptions.RefreshSecret);
            
        return Task.FromResult(new TokensModel()
        {
            AccessToken = GenerateToken(claims, accessSymmetricKey, _tokenOptions.AccessExpires),
            RefreshToken = GenerateToken(claims, refreshSymmetricKey, _tokenOptions.RefreshExpires),
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
    public Task<Claim[]?> VerifyAccessToken(string accessToken)
    {
        return Task.FromResult(ValidateToken(accessToken, Encoding.UTF8.GetBytes(_tokenOptions.AccessSecret)));   
    }
    public Task<Claim[]?> VerifyRefreshToken(string refreshToken)
    {
        return Task.FromResult(ValidateToken(refreshToken, Encoding.UTF8.GetBytes(_tokenOptions.RefreshSecret)));
    }
}