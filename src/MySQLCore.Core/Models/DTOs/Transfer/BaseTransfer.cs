namespace MySQLCore.Core.Models.DTOs;

public class BaseTransfer
{
    public ActionStatusType ActionStatusType { get; set; } = ActionStatusType.NoAction;
    public string ActionStatusName => ActionStatusType.ToString();
}