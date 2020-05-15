using System;
using System.Threading;
using System.Threading.Tasks;

namespace Joker.Contracts.Data
{
  public interface IAsyncDbTransaction : IDisposable
  {
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
  }
}