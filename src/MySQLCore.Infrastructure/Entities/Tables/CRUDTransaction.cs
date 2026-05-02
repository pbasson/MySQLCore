namespace MySQLCore.Infrastructure.Entities.Tables;

public class CRUDTransaction : BaseModel, IEntity
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
}
