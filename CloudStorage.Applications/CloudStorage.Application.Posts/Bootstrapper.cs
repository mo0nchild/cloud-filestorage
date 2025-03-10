using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CloudStorage.Application.Commons.Helpers;
using CloudStorage.Application.Commons.Models;
using CloudStorage.Application.Posts.Interfaces;
using CloudStorage.Application.Posts.Services;
using CloudStorage.Domain.Posts.Settings;

namespace CloudStorage.Application.Posts;

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