using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CloudStorage.Application.Commons.Exceptions;
using CloudStorage.Application.Users.Interfaces;
using CloudStorage.Application.Users.Models.SubscribersInfo;
using CloudStorage.Application.Users.Repositories;
using CloudStorage.Domain.Core.Factories;
using CloudStorage.Domain.Users.Entities;
using CloudStorage.Shared.Commons.Validations;

namespace CloudStorage.Application.Users.Services;

internal class SubscribersService : ISubscribersService
{
    private readonly RepositoryFactoryInterface<IUsersRepository> _repositoryFactory;
    private readonly IMapper _mapper;
    private readonly IModelValidator<NewSubscriptionInfo> _newSubscriptionInfoValidator;
    private readonly IModelValidator<RemoveSubscription> _removeSubscriptionValidator;

    public SubscribersService(RepositoryFactoryInterface<IUsersRepository> repositoryFactory,
        IModelValidator<NewSubscriptionInfo> newSubscriptionInfoValidator,
        IModelValidator<RemoveSubscription> removeSubscriptionValidator,
        IMapper mapper,
        ILogger<SubscribersService> logger)
    {
        Logger = logger;
        _newSubscriptionInfoValidator = newSubscriptionInfoValidator;
        _removeSubscriptionValidator = removeSubscriptionValidator;
        _repositoryFactory = repositoryFactory;
        _mapper = mapper;
    }
    private ILogger<SubscribersService> Logger { get; }
    
    public async Task SubscribeToUser(NewSubscriptionInfo subscriptionInfo)
    {
        await _newSubscriptionInfoValidator.CheckAsync(subscriptionInfo);
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();

        await dbContext.Subscriptions.AddRangeAsync(new Subscription()
        {
            Author = await dbContext.Users.FirstAsync(item => item.Uuid == subscriptionInfo.AuthorUuid),
            Subscriber = await dbContext.Users.FirstAsync(item => item.Uuid == subscriptionInfo.UserUuid)
        });
        await dbContext.SaveChangesAsync();
    }
    public async Task UnsubscribeFromUser(RemoveSubscription subscriptionInfo)
    {
        await _removeSubscriptionValidator.CheckAsync(subscriptionInfo);
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var subscription = await dbContext.Subscriptions.Include(item => item.Subscriber)
            .Include(item => item.Author)
            .FirstOrDefaultAsync(item => item.Author.Uuid == subscriptionInfo.AuthorUuid 
                                         && item.Subscriber.Uuid == subscriptionInfo.UserUuid);
        if (subscription == null) throw new ProcessException("Subscription was not found");
        
        dbContext.Subscriptions.RemoveRange(subscription);
        await dbContext.SaveChangesAsync();
    }
    public async Task<SubscriptionInfo> GetSubscriptionsList(Guid userUuid)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var subscriptionsList = await dbContext.Subscriptions.Include(item => item.Subscriber)
            .Include(item => item.Author)
            .Where(item => item.Subscriber.Uuid == userUuid).Select(item => item.Author).ToListAsync();
        
        return _mapper.Map<SubscriptionInfo>(subscriptionsList);
    }
    public async Task<SubscriptionInfo> GetSubscribersList(Guid userUuid)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var subscriptionsList = await dbContext.Subscriptions.Include(item => item.Subscriber)
            .Include(item => item.Author)
            .Where(item => item.Author.Uuid == userUuid).Select(item => item.Subscriber).ToListAsync();
        
        return _mapper.Map<SubscriptionInfo>(subscriptionsList);
    }
}