namespace DepoQuick.Backend.Repos;

public interface IRepo<T>
{
    public void Add(T entity);
    public List<T> GetAll();
}