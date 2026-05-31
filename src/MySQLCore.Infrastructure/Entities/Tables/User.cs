namespace MySQLCore.Infrastructure.Entities.Tables;

public class User : BaseModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public bool IsActive { get; set; } = true;
}
