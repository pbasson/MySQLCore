using MySQLCore.Core.Enums;
using MySQLCore.Core.Utilities;

namespace MySQLCore.Core.Models.DTOs;


public class TransferDTO
{
    public int Id {get; set;}
    public string Message { get; set; }
    public bool Success => Id > 0;

    public MessagerResultType type = MessagerResultType.NoAction;

    public TransferDTO(int id, string message = "", MessagerResultType type = MessagerResultType.NoAction)
    {
        var basicError = "Entity Not Created";
        var failure = !string.IsNullOrEmpty(message) ?  message : basicError ;

        Id = id;
        Message = id.IsNotZero() ? "Success: Entity Created" :   $"Failure: {failure}" ;
    }
}
