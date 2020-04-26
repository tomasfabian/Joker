namespace Joker.Contracts.Data
{
  public interface IWritableRepository<in TEntity>
  {
    void Add(TEntity entity);

    void Update(TEntity entity);

    void Remove(params object[] keys);

    void Remove(TEntity entity);
  }
}