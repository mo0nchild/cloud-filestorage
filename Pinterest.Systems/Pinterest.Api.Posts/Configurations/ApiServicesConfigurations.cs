using Pinterest.Application.Posts;
using Pinterest.Application.Tokens;
using Pinterest.Database.Posts;
using Pinterest.Shared.Commons.Configurations;

namespace Pinterest.Api.Posts.Configurations;

public static class ApiServicesConfigurations
{
    public static async Task<IServiceCollection> AddPostsApiServices(this IServiceCollection collection,
        IConfiguration configuration)
    {

        await collection.AddPostsServices();
        await collection.AddPostsDatabase(configuration);
        return collection;
    }
}