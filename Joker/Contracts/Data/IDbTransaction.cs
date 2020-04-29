using System;

namespace Joker.Contracts.Data
{
  public interface IDbTransaction : IDisposable
  {
    void Commit();
    void Rollback();
  }
}