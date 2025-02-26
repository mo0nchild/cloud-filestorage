using Microsoft.Extensions.DependencyInjection;
using Pinterest.Application.Posts.Interfaces;
using Pinterest.Application.Posts.Services;

namespace Pinterest.Application.Posts;

public static class Bootstrapper
{
    public static Task<IServiceCollection> AddPostsServices(this IServiceCollection collection)
    {
        
        return Task.FromResult(collection);
    }
}