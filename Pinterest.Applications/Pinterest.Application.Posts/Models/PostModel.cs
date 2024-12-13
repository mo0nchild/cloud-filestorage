using AutoMapper;
using Pinterest.Domain.Posts.Entities;

namespace Pinterest.Application.Posts.Models;

public class PostModel
{
    public string Title { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public Guid Uuid { get; set; } = Guid.Empty;
}

public class PostModelProfile : Profile
{
    public PostModelProfile() => CreateMap<PostInfo, PostModel>();
}