using System.Linq;

namespace Joker.Contracts.Data
{
  public interface IReadOnlyRepository<out TEntity>
  {
    IQueryable<TEntity> GetAll();
  }
}