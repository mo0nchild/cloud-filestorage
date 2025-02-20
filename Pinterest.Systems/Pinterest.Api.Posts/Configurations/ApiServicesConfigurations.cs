using Pinterest.Application.Posts;
using Pinterest.Application.Tokens;
using Pinterest.Database.Posts;
using Pinterest.S3Storage.Minio;
using Pinterest.Shared.Commons.Configurations;

namespace Pinterest.Api.Posts.Configurations;

public static class ApiServicesConfigurations
{
    public static async Task<IServiceCollection> AddAccountsApiServices(this IServiceCollection collection,
        IConfiguration configuration)
    {

        await collection.AddPostsServices();
        await collection.AddPostsDatabase(configuration);
        return collection;
    }
}