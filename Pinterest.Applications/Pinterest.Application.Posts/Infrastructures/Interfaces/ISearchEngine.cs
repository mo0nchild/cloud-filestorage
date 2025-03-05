using Pinterest.Application.Commons.Models;
using Pinterest.Domain.Core.Models;

namespace Pinterest.Application.Posts.Infrastructures.Interfaces;

public interface ISearchEngine<TModel> where TModel : BaseEntity
{
    Task IndexPostAsync(TModel postIndex);
    Task RemovePostAsync(Guid postUuid);
    Task<PagedResult<Guid>> SearchPostsAsync(SearchRequest searchRequest);
}
public class SearchRequest
{
    public required string QueryValue { get; set; }
    public required PagedRange QueryRange { get; set; }
    public IReadOnlyList<string>? Filters = default;
}