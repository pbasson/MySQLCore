using System.ComponentModel.DataAnnotations;

namespace MySQLCore.Core.Models.DTOs;

public class CRUDTransactionDTO : BaseDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public CRUDTransactionDTO() { }

}

public static class CRUDTransactionDTOFactory 
{
    public static CRUDTransactionDTO Create(int id, string? name, string? createdBy, DateTime createDateTime, string? updatedBy, DateTime updateDateTime) => new()
    {
        Id = id,
        Name = name,
        CreatedBy = createdBy,
        CreatedDateTime = createDateTime,
        UpdatedBy = updatedBy,
        UpdatedDateTime = updateDateTime,
    };

}

public class CreateCRUDTransactionDTO
{
    [Required]
    public string Name { get; set; } = "";
}

public class UpdateCRUDTransactionDTO {
    [Required]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = "";
}