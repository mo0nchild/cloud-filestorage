using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Pinterest.Api.Users.Requests;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.Users.Interfaces;
using Pinterest.Application.Users.Models;
using Pinterest.Shared.Commons.Helpers;
using Pinterest.Shared.Security.Models;
using Pinterest.Shared.Security.Settings;

namespace Pinterest.Api.Users.Controllers;

[Route("users/info"), ApiController]
public class UserInfoController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserInfoController(IUserService userService, IMapper mapper,
        ILogger<UserInfoController> logger)
    {
        Logger = logger;
        _userService = userService;
        _mapper = mapper;
    }
    private Guid UserUuid => User.GetUserUuid() ?? throw new ProcessException("User UUID not found");
    public ILogger<UserInfoController> Logger { get; }

    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("getInfo"), HttpGet]
    [ProducesResponseType(typeof(UserInfo), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUserInfo()
    {
        return Ok(await _userService.GetUserInfoAsync(UserUuid));
    }
    
    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("update"), HttpPut]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        var mappedRequest = _mapper.Map<UpdateUserInfo>(request);
        mappedRequest.UserUuid = UserUuid;
        await _userService.UpdateUserAsync(mappedRequest);
        return Ok(new { Message = "User info was been updated" });
    }

    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("update/image"), HttpPatch]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateUserImage([FromBody] UpdateImageRequest request)
    {
        var mappedRequest = _mapper.Map<UserImageInfo>(request);
        mappedRequest.UserUuid = UserUuid;
        await _userService.UpdateUserImageAsync(mappedRequest);
        return Ok(new { Message = "User image was been updated" });
    }
}