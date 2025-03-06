using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.Posts.Interfaces;
using Pinterest.Application.Posts.Models;
using Pinterest.Application.Posts.Models.PostsInfo;
using Pinterest.Shared.Commons.Helpers;
using Pinterest.Shared.Security.Infrastructure;
using Pinterest.Shared.Security.Models;
using Pinterest.Shared.Security.Settings;

namespace Pinterest.Api.Posts.Controllers;

[Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
[Route("posts"), ApiController]
public class PostsController : ControllerBase
{
    private readonly IPostsService _postsService;
    private readonly IMapper _mapper;
    public PostsController(IPostsService postsService, IMapper mapper, ILogger<PostsController> logger)
    {
        _postsService = postsService;
        _mapper = mapper;
        Logger = logger;
    }
    private ILogger<PostsController> Logger { get; }
    private Guid UserUuid => User.GetUserUuid() ?? throw new ProcessException("User Uuid not found");

    [Route("create"), HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CreatePost([FromBody] NewPostModel newPostModel)
    {
        newPostModel.AuthorUuid = UserUuid;
        await _postsService.AddPostAsync(newPostModel);
        return Ok(new { Message = "Post created successfully" });
    }
    [Route("delete"), HttpDelete]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> DeletePost([FromQuery] Guid postUuid)
    {
        await _postsService.DeletePostAsync(new RemovePostModel()
        {
            AuthorUuid = UserUuid,
            PostUuid = postUuid
        });
        return Ok(new { Message = "Post successfully deleted" });
    }
}