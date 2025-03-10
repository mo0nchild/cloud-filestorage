using System.Net;
using Microsoft.AspNetCore.Mvc;
using CloudStorage.Application.Accounts.Interfaces;
using CloudStorage.Application.Accounts.Models;

namespace CloudStorage.Api.Accounts.Controllers;

[Route("accounts"), ApiController]
public class AccountsController : ControllerBase
{
    private readonly IAccountsService _accountsService;
    public AccountsController(IAccountsService accountsService, ILogger<AccountsController> logger)
    {
        Logger = logger;
        _accountsService = accountsService;
    }
    private ILogger<AccountsController> Logger { get; }
    
    [Route("getInfo"), HttpGet]
    [ProducesResponseType(typeof(AccountModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAccountByToken([FromHeader(Name = "X-AccessToken")] string accessToken)
    {
        return Ok(await _accountsService.GetAccountByAccessToken(accessToken));
    }
    [Route("getTokens"), HttpGet]
    [ProducesResponseType(typeof(IdentityModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAuthTokens([FromQuery] CredentialsModel request)
    {
        return Ok(await _accountsService.GetTokensByCredentials(request));
    }
    [Route("getTokens/byRefresh"), HttpPatch]
    [ProducesResponseType(typeof(IdentityModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetTokensByRefresh([FromHeader(Name = "X-RefreshToken")] string refreshToken)
    {
        return Ok(await _accountsService.GetTokensByRefreshToken(refreshToken));
    }
    [Route("delete"), HttpDelete]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteAccount([FromHeader(Name = "X-AccessToken")] string accessToken)
    {
        await _accountsService.DeleteAccount(accessToken); 
        return Ok(new { Message = "Account was been deleted" });
    }
}