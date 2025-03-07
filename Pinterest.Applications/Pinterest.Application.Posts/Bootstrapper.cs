using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pinterest.Application.Commons.Helpers;
using Pinterest.Application.Commons.Models;
using Pinterest.Application.Posts.Interfaces;
using Pinterest.Application.Posts.Services;
using Pinterest.Domain.Posts.Settings;

namespace Pinterest.Application.Posts;

public static class Bootstrapper
{
    public static async Task<IServiceCollection> AddPostsServices(this IServiceCollection servicesCollection, 
        IConfiguration configuration)
    {
        servicesCollection.Configure<PostsSettings>(configuration.GetSection(nameof(PostsSettings)));
        await servicesCollection.AddInnerTransactionServices("PostsTransactions");
        
        servicesCollection.AddTransient<IPostsService, PostsService>();
        servicesCollection.AddTransient<IPostsValidators, PostsValidators>();
        servicesCollection.AddTransient<ITagsService, TagsService>();
        return servicesCollection;
    }
}