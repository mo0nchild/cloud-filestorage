using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pinterest.Api.Accounts.Requests;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.Posts.Interfaces;
using Pinterest.Application.Posts.Models;
using Pinterest.Application.Posts.Models.PostsInfo;
using Pinterest.Shared.Commons.Helpers;
using Pinterest.Shared.Security.Infrastructure;
using Pinterest.Shared.Security.Settings;

namespace Pinterest.Api.Posts.Controllers;

[Authorize(AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
[Route("posts"), ApiController]
public class PostsController : ControllerBase
{
    private readonly IMapper _mapper;

    public PostsController(IMapper mapper, ILogger<PostsController> logger)
    {
        Logger = logger;
        _mapper = mapper;
    }
    private ILogger<PostsController> Logger { get; }
    private Guid UserUuid { get => User.GetUserUuid() ?? throw new ProcessException("User Uuid not found"); }
    
    [Route("getAll"), HttpGet]
    [ProducesResponseType(typeof(List<PostModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> GetPostsByUser()
    {
        return Ok();
    }
}