namespace Joker.Contracts
{
  public interface IReactiveDataWithStatus<TEntity> : IReactiveData<TEntity>, ITableDependencyStatusProvider
    where TEntity : IVersion
  {
  }
}