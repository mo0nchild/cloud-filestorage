using Pinterest.Domain.Core.Models;

namespace Pinterest.Domain.CommonModels.Models;

public class ValidPostInfo : BaseEntity
{
    public Guid PostUuid { get; set; } = Guid.Empty;
}