using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Application.Users.Interfaces;
using CloudStorage.Application.Users.Models.FavoritePost;
using CloudStorage.Application.Users.Models.FavoritePostInfo;
using CloudStorage.Application.Users.Models.UserBasicInfo;
using CloudStorage.Shared.Commons.Helpers;
using CloudStorage.Shared.Security.Models;
using CloudStorage.Shared.Security.Settings;

namespace CloudStorage.Api.Users.Controllers;

[Route("users/favorites"), ApiController]
public class FavoritePostsController : ControllerBase
{
    private readonly IFavoritesPostService _favoritesPostService;
    private readonly IMapper _mapper;

    public FavoritePostsController(IFavoritesPostService favoritesPostService, IMapper mapper,
        ILogger<FavoritePostsController> logger)
    {
        _favoritesPostService = favoritesPostService;
        _mapper = mapper;
        Logger = logger;
    }
    private Guid UserUuid => User.GetUserUuid() ?? throw new ProcessException("User UUID not found");
    private ILogger<FavoritePostsController> Logger { get; }

    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("add"), HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> AddFavoritePost([FromQuery] Guid postUuid)
    {
        await _favoritesPostService.AddFavorite(new NewFavoriteInfo()
        {
            FavoriteUuid = postUuid,
            UserUuid = UserUuid
        });
        return Ok(new { Message = "Favorite post added successfully" });
    }
    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("remove"), HttpDelete]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> RemoveFavoritePost([FromQuery] Guid postUuid)
    {
        await _favoritesPostService.RemoveFavorite(new RemoveFavoritePost()
        {
            FavoriteUuid = postUuid,
            UserUuid = UserUuid
        });
        return Ok(new { Message = "Favorite post removed successfully" });
    }
    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("ownlist"), HttpGet]
    [ProducesResponseType(typeof(FavoriteInfo), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetFavoritePosts()
    {
        return Ok(await _favoritesPostService.GetFavoritesList(UserUuid));
    }
    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("list"), HttpGet]
    [ProducesResponseType(typeof(FavoriteInfo), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetFavoritePosts([FromQuery] Guid userUuid)
    {
        return Ok(await _favoritesPostService.GetFavoritesList(userUuid));
    }
}