using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Application.Posts.Interfaces;
using CloudStorage.Application.Posts.Models;
using CloudStorage.Application.Posts.Models.PostsInfo;
using CloudStorage.Shared.Commons.Helpers;
using CloudStorage.Shared.Security.Infrastructure;
using CloudStorage.Shared.Security.Models;
using CloudStorage.Shared.Security.Settings;

namespace CloudStorage.Api.Posts.Controllers;

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