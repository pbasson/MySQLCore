namespace MySQLCore.Core.Interfaces.InterfaceControllers.Test
{
    public interface ICRUDTransactionController_Test
    {
        Task GetAllRecords_CheckIsValue();
        Task GetAllRecords_CheckValueEmpty();
        Task GetRecordById_CheckIsValue();
        Task GetRecordById_CheckValueEmpty();
        Task CreateRecord_CheckIsValue();
        Task UpdateRecord_CheckIsValue();
        Task DeleteRecord_CheckIsValue();
    }
}