using System;
using Joker.MVVM.Enums;

namespace Joker.MVVM.Contracts
{
  public interface IReactive<TEntity>
    where TEntity : IVersion
  {
    IObservable<EntityChange<TEntity>> WhenDataChanges { get; }
  }
}