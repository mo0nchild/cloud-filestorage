using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Application.Users.Infrastructures;
using CloudStorage.Application.Users.Interfaces;
using CloudStorage.Application.Users.Models;
using CloudStorage.Application.Users.Models.UserBasicInfo;
using CloudStorage.Application.Users.Repositories;
using CloudStorage.Domain.Core.Factories;
using CloudStorage.Domain.Core.MessageBus;
using CloudStorage.Domain.Messages.FileStorageMessages;
using CloudStorage.Domain.Messages.UsersMessages;
using CloudStorage.Domain.Users.Entities;
using CloudStorage.Shared.Commons.Validations;

namespace CloudStorage.Application.Users.Services;

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

        await _messageProducer.SendToAllAsync(CreatedUserMessage.RoutingPath, new CreatedUserMessage()
        {
            UserUuid = mappedUserInfo.Uuid,
        });
        return mappedUserInfo.Uuid;
    }
    public async Task DeleteUserAsync(Guid userUuid)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Uuid == userUuid);
        if (user == null) throw new ProcessException("User not found");

        dbContext.Users.RemoveRange(user);
        await dbContext.SaveChangesAsync();
        await _messageProducer.SendToAllAsync(RemoveUserMessage.RoutingPath, new RemoveUserMessage()
        {
            UserUuid = user.Uuid
        });
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