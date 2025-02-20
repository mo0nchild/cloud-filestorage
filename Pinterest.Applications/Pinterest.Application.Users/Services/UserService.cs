using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.Users.Infrastructures;
using Pinterest.Application.Users.Interfaces;
using Pinterest.Application.Users.Models;
using Pinterest.Application.Users.Repositories;
using Pinterest.Domain.Core.Factories;
using Pinterest.Domain.Core.MessageBus;
using Pinterest.Domain.Messages.FileStorageMessages;
using Pinterest.Domain.Users.Entities;
using Pinterest.Shared.Commons.Validations;

namespace Pinterest.Application.Users.Services;

internal class UserService : IUserService
{
    private readonly RepositoryFactoryInterface<IUsersRepository> _repositoryFactory;
    private readonly IMapper _mapper;
    private readonly IReserveUserImage _reserveUserImage;
    private readonly IMessageProducer _messageProducer;
    private readonly IUserValidators _userValidators;

    public UserService(RepositoryFactoryInterface<IUsersRepository> repositoryFactory,
        IMapper mapper,
        IReserveUserImage reserveUserImage,
        IMessageProducer messageProducer,
        IUserValidators userValidators,
        ILogger<UserService> logger)
    {
        _repositoryFactory = repositoryFactory;
        _mapper = mapper;
        _reserveUserImage = reserveUserImage;
        _messageProducer = messageProducer;
        _userValidators = userValidators;
        Logger = logger;
    }
    public ILogger<UserService> Logger { get; }
    
    public async Task<UserInfo> GetUserInfoAsync(Guid userUuid)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Uuid == userUuid) 
                    ?? throw new ProcessException("User not found");
        return _mapper.Map<UserInfo>(user);
    }
    public async Task<Guid> CreateUserAsync(NewUserInfo newUserInfo)
    {
        await _userValidators.NewUserValidator.CheckAsync(newUserInfo);
        var mappedUserInfo = _mapper.Map<User>(newUserInfo);
        
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        await dbContext.Users.AddRangeAsync(mappedUserInfo);
        await dbContext.SaveChangesAsync();
        
        return mappedUserInfo.Uuid;
    }
    public async Task DeleteUserAsync(Guid userUuid)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Uuid == userUuid);
        if (user == null) throw new ProcessException("User not found");

        dbContext.Users.RemoveRange(user);
        await dbContext.SaveChangesAsync();
    }
    public async Task UpdateUserAsync(UpdateUserInfo userInfo)
    {
        await _userValidators.UpdateUserValidator.CheckAsync(userInfo);
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Uuid == userInfo.UserUuid);
        if (user == null) throw new ProcessException("User not found");
        
        user.UserThemes = userInfo.UserThemes.ToList();
        user.Username = userInfo.Username;
        
        await dbContext.SaveChangesAsync();
    }
    public async Task UpdateUserImageAsync(UserImageInfo userImage)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Uuid == userImage.UserUuid);
        if (user == null) throw new ProcessException("User not found");
        
        if (!userImage.ImageUuid.HasValue)
        {
            ProcessException.ThrowIf(() => user.PhotoPath == null, "User photo not found");
            
            var currentImageUuid = Guid.Parse(user.PhotoPath!);
            user.PhotoPath = null;
            
            await dbContext.SaveChangesAsync();
            await _messageProducer.SendAsync(FileRemovedMessage.RoutingPath, new FileRemovedMessage()
            {
                FileUuid = currentImageUuid,
            });
            return;
        }
        await _reserveUserImage.ReserveUserImageAsync(userImage.ImageUuid.Value);
        
        user.PhotoPath = userImage.ImageUuid.Value.ToString();
        await dbContext.SaveChangesAsync();
    }
}