namespace MySQLCore.Core.Models.DTOs.User;

public class UserDTO : BaseDTO, IEntity
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public bool IsActive { get; set; }

    public UserDTO() { }
}

public class CreateUserDTO
{
    [Required]
    public string? UserName { get; set; }
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}

public class UpdateUserDTO : IEntity 
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string? UserName { get; set; }
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}

public class UserTransferGridDTO : BaseTransfer
{
    public List<UserDTO> Records { get; set; }

    public UserTransferGridDTO(List<UserDTO> Records) 
    {
        this.Records = Records;
    }

    public int TotalRecords => Records?.Count ?? 0;
}

public class UserTransferDTO : BaseTransfer
{
    public UserDTO Record { get; set; }

    public UserTransferDTO(UserDTO Record) 
    {
        // this.ActionStatusType = ActionStatusType;
        this.Record = Record;
    }
}