using System.Threading;
using System.Threading.Tasks;

namespace Joker.Contracts.Data
{
  public interface IContext
  {
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
  }
}