using System.Net;
using Microsoft.AspNetCore.Mvc;
using Pinterest.Application.Accounts.Interfaces;
using Pinterest.Application.Accounts.Models;

namespace Pinterest.Api.Accounts.Controllers;

[Route("users"), ApiController]
public class AccountsController : ControllerBase
{
    private readonly IAccountsService _accountsService;
    public ILogger<AccountsController> Logger { get; }

    public AccountsController(IAccountsService accountsService, ILogger<AccountsController> logger)
    {
        Logger = logger;
        _accountsService = accountsService;
    }
    [Route("getInfo"), HttpGet]
    [ProducesResponseType(typeof(AccountModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAccountByToken([FromQuery] string accessToken)
    {
        return Ok(await _accountsService.GetAccountByAccessToken(accessToken));
    }
    [Route("registrate"), HttpPost]
    [ProducesResponseType(typeof(IdentityModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> RegistrateAccount([FromBody] RegistrationModel request)
    {
        return Ok(await _accountsService.Registration(request));
    }
    [Route("getTokens"), HttpGet]
    [ProducesResponseType(typeof(AccountModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAuthTokens([FromQuery] CredentialsModel request)
    {
        return Ok(await _accountsService.GetTokensByCredentials(request));
    }
}