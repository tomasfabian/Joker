using Joker.MVVM.Contracts;

namespace Joker.MVVM.Enums
{
  public class EntityChange<TEntity>
    where TEntity : IVersion
  {
    public TEntity Entity { get; set; }
    public ChangeType ChangeType { get; set; }
  }
}