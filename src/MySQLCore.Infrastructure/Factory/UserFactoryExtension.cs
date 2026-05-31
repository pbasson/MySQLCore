namespace MySQLCore.Infrastructure.Factory;

public static class UserFactoryExtension
{
    public static User ToEntity(this UserDTO dto) => new()
    {
        Id = dto.Id,
        UserName = "",
        FirstName = "",
        LastName = "",
        DateOfBirth = new DateOnly(2000, 1, 1),
        CreatedBy = dto.CreatedBy,
        CreatedDateTime = dto.CreatedDateTime,
        UpdatedBy = dto.UpdatedBy,
        UpdatedDateTime = dto.UpdatedDateTime,
    };

    public static User ToEntity(this CreateUserDTO dto) => new()
    {
        UserName = dto.UserName ?? string.Empty,
        FirstName = dto.FirstName ?? string.Empty,
        LastName = dto.LastName ?? string.Empty,
        DateOfBirth = dto.DateOfBirth ,
        Email = dto.Email! ?? string.Empty,
    };

    public static User ToEntity(this UpdateUserDTO dto) => new()
    {
        Id = dto.Id,
        UserName = dto.UserName ?? string.Empty,
        Email = dto.Email! ?? string.Empty,
    };

    public static UserDTO ToMapped(this User dto) => new()
    {
        Id = dto.Id,
        UserName = dto.UserName,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        CreatedBy = dto.CreatedBy,
        CreatedDateTime = dto.CreatedDateTime,
        UpdatedBy = dto.UpdatedBy,
        UpdatedDateTime = dto.UpdatedDateTime,
    };
}

