using DepoQuick.Models;

namespace DepoQuick.DataAccess.Repos;

public interface IRepo<TEntity, TKey>
{
    public List<TEntity> GetAll();
    public TEntity? Get(TKey id);
    public TEntity Add(TEntity entity);
    public TEntity Update(TEntity entity);
    public void Delete(TKey id);
}