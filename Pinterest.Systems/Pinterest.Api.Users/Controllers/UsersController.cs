using System.Net;
using Microsoft.AspNetCore.Mvc;
using Pinterest.Application.Users.Interfaces;
using Pinterest.Application.Users.Models;
using Pinterest.Domain.Users.Enums;

namespace Pinterest.Api.Users.Controllers;

[Route("users"), ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) : base()
    {
        _userService = userService;
    }

    [Route("getAll"), HttpGet]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAllUsers()
    {
        await this._userService.Registrate(new RegistrationModel()
        {
            Email = "test@gmail.com",
            Username = "asdasdasd",
            Gender = Gender.Male,
            Password = "1231234",
        });
        return this.Ok("All users");
    }
    
}