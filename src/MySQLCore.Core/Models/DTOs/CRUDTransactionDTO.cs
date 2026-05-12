using System.ComponentModel.DataAnnotations;

namespace MySQLCore.Core.Models.DTOs;

public class CRUDTransactionDTO : BaseDTO, IEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public CRUDTransactionDTO() { }

}

public class CreateCRUDTransactionDTO
{
    [Required]
    public string Name { get; set; } = "";
}

public class UpdateCRUDTransactionDTO : IEntity 
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = "";
}