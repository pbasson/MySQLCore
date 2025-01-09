namespace MySQLCore.Infrastructure.Entities.Tables;

public class BaseModel
{
    public DateTime CreatedDateTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public string? UpdatedBy { get; set; }
}
