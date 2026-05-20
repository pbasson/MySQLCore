using MySQLCore.Core.Utilities;

namespace MySQLCore.Core.Models.DTOs;


public class TransferDTO
{
    public int Id {get; set;}
    public Guid? MessageId {get; set;} 
    public string Message { get; set; }
    public bool Success => Id > 0;

    public ServiceResultType ServiceResultType = ServiceResultType.NoAction;

    public TransferDTO(int id, string message = "", ServiceResultType serviceResultType = ServiceResultType.NoAction, Guid? messageId = null)
    {
        var basicError = "Entity Not Created";
        var failure = !string.IsNullOrEmpty(message) ?  message : basicError ;

        Id = id;
        MessageId = messageId;
        Message = id.IsNotZero() ? "Success: Entity Created" :   $"Failure: {failure}" ;
        ServiceResultType = serviceResultType;
    }
}

public static class TransferFactory
{
    public static TransferDTO GetTransferFailure(TransferEnum transfer)
    {
        var message = "Data Error";
        switch (transfer)
        {
            case TransferEnum.DTONull:
                message = "DTO Is Null";
                break;
            case TransferEnum.EntityNotCreated:
                message = "Entity Not Created";
                break;
            case TransferEnum.EntityNotExist:
                message = "Entity Does Not Exist";
                break;
            case TransferEnum.SaveChangesNotExecuted:
                message = "Save Changes Not Executed";
                break;

        }
        return new TransferDTO(0, message, ServiceResultType.Failed);
    }
}

public enum TransferEnum
{
    DTONull = 0,
    EntityNotCreated = 1,
    SaveChangesNotExecuted = 2,
    EntityNotExist = 3,
}
