namespace DaemonsRunner.DAL.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    public IEnumerable<T> GetAll();

    public void Insert(T entity);
}
