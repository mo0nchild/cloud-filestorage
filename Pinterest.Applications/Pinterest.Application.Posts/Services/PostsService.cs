using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pinterest.Application.Commons.Exceptions;
using Pinterest.Application.Commons.S3Storage;
using Pinterest.Application.Posts.Interfaces;
using Pinterest.Application.Posts.Models;
using Pinterest.Application.Posts.Repositories;
using Pinterest.Domain.Core.Factories;
using Pinterest.Domain.Posts.Entities;
using Pinterest.Shared.Commons.Validations;

namespace Pinterest.Application.Posts.Services;

public class PostsService : IPostsService
{
    private readonly RepositoryFactoryInterface<IPostsRepository> _contextFactory;
    private readonly IMapper _mapper;
    private readonly IS3StorageService _s3StorageService;
    private readonly IModelValidator<NewPostModel> _newPostModelValidator;

    public PostsService(RepositoryFactoryInterface<IPostsRepository> contextFactory, 
        IS3StorageService s3StorageService,
        IMapper mapper, 
        IModelValidator<NewPostModel> newPostModelValidator)
    {
        _contextFactory = contextFactory;
        _mapper = mapper;
        _s3StorageService = s3StorageService;
        _newPostModelValidator = newPostModelValidator;
    }
    public async Task<List<PostModel>> GetPostsAsync(Guid userUuid)
    {
        using var dbContext = await _contextFactory.CreateRepositoryAsync();
        var result = await dbContext.Posts
            .Where(item => item.UserUuid == userUuid).ToListAsync();
        var mappedPosts = _mapper.Map<List<PostModel>>(result);
        foreach (var post in mappedPosts)
        {
            var fileUrl = await _s3StorageService.GetObjectUrlFromStorage(new BucketInfo()
            {
                BucketName = "images",
                ObjectName = result.FirstOrDefault(item => item.Uuid == post.Uuid)?.FileName ?? "",
            }, DateTime.Now.AddDays(2));
            post.FilePath = fileUrl ?? "";
        }
        return mappedPosts;
    }
    public async Task AddPostAsync(NewPostModel newPost)
    {
        await _newPostModelValidator.CheckAsync(newPost);
        var mappedPost = _mapper.Map<PostInfo>(newPost);
        mappedPost.FileName = $"{Guid.NewGuid()}.png";
        var response = await _s3StorageService.LoadObjectToStorage(newPost.FileContent, new BucketInfo()
        {
            BucketName = "images",
            ObjectName = mappedPost.FileName
        });
        ProcessException.ThrowIf(() => !response, "Failed to load post file");
        using var dbContext = await _contextFactory.CreateRepositoryAsync();
        
        await dbContext.Posts.AddRangeAsync(mappedPost);
        await dbContext.SaveChangesAsync();
    }
}