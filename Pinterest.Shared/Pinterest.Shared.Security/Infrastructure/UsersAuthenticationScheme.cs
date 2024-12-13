using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Pinterest.Application.Tokens.Interfaces;
using Pinterest.Shared.Security.Settings;

namespace Pinterest.Shared.Security.Infrastructure;

public class UsersAuthenticationScheme : AuthenticationHandler<UsersAuthenticationOptions>
{
    private readonly ITokenService _tokenService;

    public UsersAuthenticationScheme(IOptionsMonitor<UsersAuthenticationOptions> options, 
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ITokenService tokenService) : base(options, logger, encoder, clock)
    {
        _tokenService = tokenService;
    }
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = Request.Headers["X-Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            return AuthenticateResult.Fail("Authentication header validation error");
        }
        var tokenValue = authorizationHeader.Split(' ')[1];
        
        var claims = await _tokenService.VerifyAccessToken(tokenValue);
        if (claims == null) return AuthenticateResult.Fail("Token verification error");
        
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}