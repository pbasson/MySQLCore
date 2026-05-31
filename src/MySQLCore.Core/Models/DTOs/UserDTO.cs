namespace MySQLCore.Core.Models.DTOs;

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
    [EmailAddress]
    public string? Email { get; set; }
}

public class UserTransferGridDTO : BaseTransfer
{
    public List<UserDTO> Records { get; set; }

    public UserTransferGridDTO( ActionStatusType ActionStatusType, List<UserDTO> Records) 
    {
        this.ActionStatusType = ActionStatusType;
        this.Records = Records;
    }
}

public class UserTransferDTO : BaseTransfer
{
    public UserDTO Record { get; set; }

    public UserTransferDTO( ActionStatusType ActionStatusType, UserDTO Record) 
    {
        this.ActionStatusType = ActionStatusType;
        this.Record = Record;
    }
}