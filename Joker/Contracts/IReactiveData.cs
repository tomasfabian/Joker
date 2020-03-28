using System;
using Joker.Enums;

namespace Joker.Contracts
{
  public interface IReactiveData<TEntity>
    where TEntity : IVersion
  {
    IObservable<EntityChange<TEntity>> WhenDataChanges { get; }
  }
}