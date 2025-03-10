using CloudStorage.Domain.Core.Models;

namespace CloudStorage.Domain.CommonModels.Models;

public class ValidPostInfo : BaseEntity
{
    public Guid PostUuid { get; set; } = Guid.Empty;
}