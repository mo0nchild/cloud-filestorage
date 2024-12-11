using Pinterest.Application.Users.Interfaces;
using Pinterest.Application.Users.Models;
using Pinterest.Application.Users.Repositories;
using Pinterest.Domain.Core.Factories;
using Pinterest.Domain.Core.Models;
using Pinterest.Domain.Users.Entities;

namespace Pinterest.Application.Users.Services;

public class UserService : IUserService
{
    private readonly RepositoryFactoryInterface<IUserDbContext> _factory;

    public UserService(RepositoryFactoryInterface<IUserDbContext> factory) : base()
    {
        _factory = factory;
    }
    public async Task Registrate(RegistrationModel info)
    {
        using var context = await _factory.CreateRepositoryAsync();
        await context.Users.AddAsync(new User()
        {
            Uuid = Guid.NewGuid(),
            Status = EntityStatus.Processing,
            Email = info.Email,
            Gender = info.Gender,
            Username = info.Username,
            CreatedTime = DateTime.UtcNow,
        });
        await context.SaveChangesAsync();
    }
    public async Task<UserInfo> GetUserInfo(Guid userUuid)
    {
        using var context = await _factory.CreateRepositoryAsync();
        var user = await context.Users.FindAsync(userUuid);
        return new UserInfo()
        {
            
        };
    }
}