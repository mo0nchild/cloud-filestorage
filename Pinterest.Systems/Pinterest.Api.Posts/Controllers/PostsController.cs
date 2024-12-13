using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pinterest.Api.Accounts.Requests;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.Posts.Interfaces;
using Pinterest.Application.Posts.Models;
using Pinterest.Shared.Commons.Helpers;
using Pinterest.Shared.Security.Infrastructure;
using Pinterest.Shared.Security.Settings;

namespace Pinterest.Api.Posts.Controllers;

[Route("posts"), ApiController]
public class PostsController : ControllerBase
{
    private readonly IPostsService _postsService;
    private readonly IMapper _mapper;
    public ILogger<PostsController> Logger { get; }
    protected Guid UserUuid { get => User.GetUserUuid() ?? throw new ProcessException("User Uuid not found"); }

    public PostsController(IPostsService postsService, IMapper mapper,
        ILogger<PostsController> logger)
    {
        Logger = logger;
        _mapper = mapper;
        _postsService = postsService;
    }
    [Authorize(AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("getAll"), HttpGet]
    [ProducesResponseType(typeof(List<PostModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> GetPostsByUser()
    {
        return Ok(await _postsService.GetPostsAsync(UserUuid));
    }
    [Authorize(AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("add"), HttpPost]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [RequestSizeLimit(200_000_000)]
    [RequestFormLimits(ValueCountLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<ActionResult> AddPost([FromForm] NewPostRequest request)
    {
        using var memoryStream = new MemoryStream();
        await request.FileContent.CopyToAsync(memoryStream);
        
        await _postsService.AddPostAsync(new NewPostModel()
        {
            Title = request.Title,
            UserUuid = UserUuid,
            FileContent = memoryStream.ToArray()
        });
        return Ok("Successfully added post");
    }
}