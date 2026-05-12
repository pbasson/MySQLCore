namespace MySQLCore.Core.Interfaces.Repos;

public interface IRepository<T> where T : IEntity
{
    Task<T> GetById(int id);

    Task Upsert(T entity);

    Task Delete(int id);
}