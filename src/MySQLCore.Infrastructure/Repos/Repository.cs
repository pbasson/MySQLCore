namespace MySQLCore.Infrastructure.Repos;

public class Repository<T> : IRepository<T> where T : IEntity
{
    private readonly object _lockObject = new();
    private List<T> Store { get; } = [];
    public readonly MySQLCoreDBContext _dBContext;
    
    public Repository(MySQLCoreDBContext dBContext)
    {
        _dBContext = dBContext;
    }

 
    public Task<T> GetById(int id)
    {
        lock (_lockObject)
        {
            var result = Store.FirstOrDefault(x => x.Id == id);

            // Passing ref, need to clone object.
            return Task.FromResult(result!);
        }
    }

    public Task Upsert(T entity)
    {
        throw new NotImplementedException();
    }
   
    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }
}

