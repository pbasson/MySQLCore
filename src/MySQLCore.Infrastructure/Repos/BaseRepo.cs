namespace MySQLCore.Infrastructure.Repos;

public abstract class BaseRepo<T>(MySQLCoreDBContext dBContext, ILogger<T> logger) where T : IBaseRepo
{
    public readonly MySQLCoreDBContext _dBContext = dBContext;
    public readonly SemaphoreSlim _semaphore = new(1, 1);
    public ILogger<T> _logger = logger;


    protected void UpdateEntity(object existDTO, object mapped)
    {
        _dBContext.Entry(existDTO).State = EntityState.Detached;
        _dBContext.Entry(mapped).State = EntityState.Modified;
    }

    protected async Task<bool> SaveChangesAsync(CancellationToken cancellationToken) {
        var result = await _dBContext.SaveChangesAsync(cancellationToken);
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
