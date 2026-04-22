namespace MySQLCore.Infrastructure.Factory;

public class CRUDFactory
{
    public CRUDTransaction ToEntity(CRUDTransactionDTO dto)
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

    public CRUDTransaction ToEntity(CreateCRUDTransactionDTO dto)
    {
        return new CRUDTransaction()
        {
            Name = dto.Name,
        };
    }  

    public CRUDTransaction ToEntity(UpdateCRUDTransactionDTO dto)
    {
        return new CRUDTransaction()
        {
            Id = dto.Id,
            Name = dto.Name,
        };
    } 

    public CRUDTransactionDTO ToMapped(CRUDTransaction dto)
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

