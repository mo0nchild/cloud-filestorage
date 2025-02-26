using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.Users.Interfaces;
using Pinterest.Shared.Contracts;
using Pinterest.Shared.Contracts.Subscriptions;
using Pinterest.Shared.Security.Models;
using Pinterest.Shared.Security.Settings;

namespace Pinterest.Api.Users.Services;

public class AuthorSubscribersServiceImpl : AuthorSubscribersService.AuthorSubscribersServiceBase
{
    private readonly ISubscribersService _subscribersService;
    
    public AuthorSubscribersServiceImpl(ISubscribersService subscribersService, ILogger<AuthorSubscribersServiceImpl> logger)
    {
        Logger = logger;
        _subscribersService = subscribersService;
    }
    private ILogger<AuthorSubscribersServiceImpl> Logger { get; }
    
    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    public override async Task<SubscribersResult> GetSubscribers(AuthorInfo request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.AuthorUuid, out var authorUuid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid GUID"));
        }
        var subscribersResult = new SubscribersResult();
        try {
            var subscribers = await _subscribersService.GetSubscribersList(authorUuid);
            var mappedResult = subscribers.Users.Select(it => new SubscriberInfo() { UserUuid = it.Uuid.ToString() });
            subscribersResult.Subscribers.AddRange(mappedResult.ToList());
        }
        catch (ProcessException error)
        {
            Logger.LogError($"Cannot get subscribers for {request.AuthorUuid}: {error.Message}");
            throw new RpcException(new Status(StatusCode.NotFound, error.Message));
        }
        return subscribersResult;
    }
    [Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    public override async Task GetSubscribersStream(AuthorInfo request, IServerStreamWriter<SubscriberInfo> responseStream, 
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.AuthorUuid, out var authorUuid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid GUID"));
        }
        try {
            var subscribers = await _subscribersService.GetSubscribersList(authorUuid);
            foreach (var item in subscribers.Users.Select(it => new SubscriberInfo { UserUuid = it.Uuid.ToString() }))
            {
                await responseStream.WriteAsync(item);
            }
        }
        catch (ProcessException error)
        {
            Logger.LogError($"Cannot get subscribers for {request.AuthorUuid}: {error.Message}");
            throw new RpcException(new Status(StatusCode.NotFound, error.Message));
        }
    }
}