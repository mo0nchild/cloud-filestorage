using CloudStorage.Domain.Core.Models;

namespace CloudStorage.Domain.CommonModels.Models;

public class ValidUserInfo : BaseEntity
{
    public Guid UserUuid { get; set; } = Guid.Empty;
}