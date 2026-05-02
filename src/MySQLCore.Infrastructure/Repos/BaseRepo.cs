namespace MySQLCore.Infrastructure.Repos;

public class BaseRepo(MySQLCoreDBContext dBContext)
{
    public readonly MySQLCoreDBContext _dBContext = dBContext;
    public readonly SemaphoreSlim _semaphore = new(1, 1);

    public void UpdateEntity(object existDTO, object mapped)
    {
        _dBContext.Entry(existDTO).State = EntityState.Detached;
        _dBContext.Entry(mapped).State = EntityState.Modified;
    }

    public async Task<bool> SaveChangesAsync() {
        var result = await _dBContext.SaveChangesAsync();
        return result > 0;
    } 
}
