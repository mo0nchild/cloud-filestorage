using CloudStorage.Application.Users.Models.SubscribersInfo;

namespace CloudStorage.Application.Users.Interfaces;

public interface ISubscribersService
{
    Task SubscribeToUser(NewSubscriptionInfo subscriptionInfo);
    Task UnsubscribeFromUser(RemoveSubscription subscriptionInfo);
    
    Task<SubscriptionInfo> GetSubscriptionsList(Guid userUuid);
    Task<SubscriptionInfo> GetSubscribersList(Guid userUuid);
}