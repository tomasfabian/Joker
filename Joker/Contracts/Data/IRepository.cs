namespace Joker.Contracts.Data
{
  public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>, IWritableRepository<TEntity>, IContext
  {
  }
}