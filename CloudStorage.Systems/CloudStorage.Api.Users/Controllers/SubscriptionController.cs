using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Application.Users.Interfaces;
using CloudStorage.Application.Users.Models.SubscribersInfo;
using CloudStorage.Shared.Commons.Helpers;
using CloudStorage.Shared.Security.Models;
using CloudStorage.Shared.Security.Settings;

namespace CloudStorage.Api.Users.Controllers;

[Route("users/subscription"), ApiController]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscribersService _subscribersService;
    private readonly IMapper _mapper;

    public SubscriptionController(ISubscribersService subscribersService, IMapper mapper, 
        ILogger<SubscriptionController> logger)
    {
        Logger = logger;
        _subscribersService = subscribersService;
        _mapper = mapper;
    }
    private Guid UserUuid => User.GetUserUuid() ?? throw new ProcessException("User UUID not found");
    private ILogger<SubscriptionController> Logger { get; }

    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("add"), HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SubscribeToUser([FromQuery] Guid authorUuid)
    {
        await _subscribersService.SubscribeToUser(new NewSubscriptionInfo()
        {
            AuthorUuid = authorUuid,
            UserUuid = UserUuid
        });
        return Ok(new { Message = "Subscription submitted successfully" });
    }
    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("remove"), HttpDelete]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UnsubscribeFromUser([FromQuery] Guid authorUuid)
    {
        await _subscribersService.UnsubscribeFromUser(new RemoveSubscription()
        {
            AuthorUuid = authorUuid,
            UserUuid = UserUuid
        });
        return Ok(new { Message = "Subscription removed successfully" });
    }
    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("ownlist"), HttpGet]
    [ProducesResponseType(typeof(SubscriptionInfo), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserSubscriptions()
    {
        return Ok(await _subscribersService.GetSubscriptionsList(UserUuid));
    }
    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("list"), HttpGet]
    [ProducesResponseType(typeof(SubscriptionInfo), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserSubscriptions([FromQuery] Guid userUuid)
    {
        return Ok(await _subscribersService.GetSubscriptionsList(userUuid));
    }
    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("/subscribers/ownlist"), HttpGet]
    [ProducesResponseType(typeof(SubscriptionInfo), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserSubscribers()
    {
        return Ok(await _subscribersService.GetSubscriptionsList(UserUuid));
    }
    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("/subscribers/list"), HttpGet]
    [ProducesResponseType(typeof(SubscriptionInfo), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserSubscribers([FromQuery] Guid userUuid)
    {
        return Ok(await _subscribersService.GetSubscriptionsList(userUuid));
    }
}