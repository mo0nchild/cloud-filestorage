using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pinterest.Domain.Core.Models;

public class BaseEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public virtual Guid Uuid { get; set; } = Guid.NewGuid();
    
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}