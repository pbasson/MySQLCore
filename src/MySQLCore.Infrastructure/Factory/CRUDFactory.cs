namespace MySQLCore.Infrastructure.Factory;

public class CRUDFactory
{
    public CRUDTransaction Create(CRUDTransactionDTO dto)
    {
        return new CRUDTransaction()
        {
            Id = dto.Id,
            Name = dto.Name,
            CreatedBy = dto.CreatedBy,
            CreatedDateTime = dto.CreatedDateTime,
            UpdatedBy = dto.UpdatedBy,
            UpdatedDateTime = dto.UpdatedDateTime,
        };
    }  

    public CRUDTransaction Create(CreateCRUDTransactionDTO dto)
    {
        return new CRUDTransaction()
        {
            Name = dto.Name,
        };
    }  

    public CRUDTransaction Create(UpdateCRUDTransactionDTO dto)
    {
        return new CRUDTransaction()
        {
            Id = dto.Id,
            Name = dto.Name,
        };
    } 

    public CRUDTransactionDTO Mapped(CRUDTransaction dto)
    {
        return new CRUDTransactionDTO()
        {
            Id = dto.Id,
            Name = dto.Name,
            CreatedBy = dto.CreatedBy,
            CreatedDateTime = dto.CreatedDateTime,
            UpdatedBy = dto.UpdatedBy,
            UpdatedDateTime = dto.UpdatedDateTime,
        };
    }  
}

