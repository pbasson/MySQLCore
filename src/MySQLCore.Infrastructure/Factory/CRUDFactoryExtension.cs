namespace MySQLCore.Infrastructure.Factory;

public static class CRUDFactoryExtension
{
    public static CRUDTransaction ToEntity(this CRUDTransactionDTO dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        CreatedBy = dto.CreatedBy,
        CreatedDateTime = dto.CreatedDateTime,
        UpdatedBy = dto.UpdatedBy,
        UpdatedDateTime = dto.UpdatedDateTime,
    };

    public static CRUDTransaction ToEntity(this CreateCRUDTransactionDTO dto) => new()
    {
        Name = dto.Name,
    };

    public static CRUDTransaction ToEntity(this UpdateCRUDTransactionDTO dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
    };

    public static CRUDTransactionDTO ToMapped(this CRUDTransaction dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        CreatedBy = dto.CreatedBy,
        CreatedDateTime = dto.CreatedDateTime,
        UpdatedBy = dto.UpdatedBy,
        UpdatedDateTime = dto.UpdatedDateTime,
    };
}

