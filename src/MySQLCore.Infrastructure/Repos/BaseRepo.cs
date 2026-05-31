namespace MySQLCore.Infrastructure.Repos;

public class BaseRepo(MySQLCoreDBContext dBContext)
{
    public readonly MySQLCoreDBContext _dBContext = dBContext;
    public readonly SemaphoreSlim _semaphore = new(1, 1);

    protected void UpdateEntity(object existDTO, object mapped)
    {
        _dBContext.Entry(existDTO).State = EntityState.Detached;
        _dBContext.Entry(mapped).State = EntityState.Modified;
    }

    protected async Task<bool> SaveChangesAsync() {
        var result = await _dBContext.SaveChangesAsync();
        return result > 0;
    } 


    protected bool IsDuplicateKeyException(DbUpdateException ex)
    {
        // Check if the exception is a MySQL duplicate key error
        if (ex.InnerException is MySqlConnector.MySqlException mysqlEx)
        {
            return mysqlEx.Number == 1062; // MySQL error code for duplicate entry
        }
        return false;
    }
}
