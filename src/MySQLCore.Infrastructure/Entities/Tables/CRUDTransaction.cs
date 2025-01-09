using System.ComponentModel.DataAnnotations;

namespace MySQLCore.Infrastructure.Entities.Tables;

public class CRUDTransaction : BaseModel
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
}
