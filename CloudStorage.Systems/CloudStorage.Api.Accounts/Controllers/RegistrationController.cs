using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using CloudStorage.Application.Accounts.Interfaces;
using CloudStorage.Application.Accounts.Models;
using CloudStorage.Application.Accounts.Sagas;
using CloudStorage.Domain.Core.Transactions;

namespace CloudStorage.Api.Accounts.Controllers;

[Route("accounts/registration"), ApiController]
public class RegistrationController : ControllerBase
{
    private readonly IRegistrateAccount _registrateAccount;

    public RegistrationController(IRegistrateAccount registrateAccount, ILogger<RegistrationController> logger)
    {
        Logger = logger;
        _registrateAccount = registrateAccount;
    }
    public ILogger<RegistrationController> Logger { get; }

    [Route("create"), HttpPost]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateAccount([FromBody] RegistrationModel registrationModel)
    {
        return Ok(await _registrateAccount.RegistrateAccountAsync(registrationModel));
    }
    
    [Route("check"), HttpGet]
    [ProducesResponseType(typeof(AccountSagaInfo), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CheckRegistration([FromQuery] Guid registrationUuid)
    {
        return Ok(await _registrateAccount.GetRegistrateAccountStateAsync(registrationUuid));
    } 
}