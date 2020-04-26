namespace Joker.Contracts.Data
{
  public interface IWritableRepository<in TEntity>
  {
    void Add(TEntity entity);

    void Update(TEntity entity);

    void Remove(int key); //TODO: support other key types

    void Remove(TEntity entity);
  }
}