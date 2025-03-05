using Pinterest.Domain.Core.Models;

namespace Pinterest.Domain.CommonModels.Models;

public class ValidUserInfo : BaseEntity
{
    public Guid UserUuid { get; set; } = Guid.Empty;
}