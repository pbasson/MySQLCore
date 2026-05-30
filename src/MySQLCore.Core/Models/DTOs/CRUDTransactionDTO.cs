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

public class TransferCRUDTransactionGridDTO : BaseTransfer
{
    public List<CRUDTransactionDTO> Records { get; set; }

    public TransferCRUDTransactionGridDTO( ActionStatusType ActionStatusType, List<CRUDTransactionDTO> Records) 
    {
        this.ActionStatusType = ActionStatusType;
        this.Records = Records;
    }
}

public class TransferCRUDTransactionDTO : BaseTransfer
{
    public CRUDTransactionDTO Record { get; set; }

    public TransferCRUDTransactionDTO( ActionStatusType ActionStatusType, CRUDTransactionDTO Record) 
    {
        this.ActionStatusType = ActionStatusType;
        this.Record = Record;
    }
}