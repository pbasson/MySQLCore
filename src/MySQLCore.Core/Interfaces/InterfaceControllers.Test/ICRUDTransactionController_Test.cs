namespace MySQLCore.Core.Interfaces.InterfaceControllers.Test
{
    public interface ICRUDTransactionController_Test
    {
        Task GetAllRecords_CheckIsValue();
        Task GetRecordById_CheckIsValue();
        Task CreateRecord_CheckIsValue();
        Task UpdateRecord_CheckIsValue();
        Task DeleteRecord_CheckIsValue();
    }
}