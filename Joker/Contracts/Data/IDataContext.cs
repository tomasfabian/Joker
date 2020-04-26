using System.Threading.Tasks;

namespace Joker.Contracts.Data
{
  public interface IContext
  {
    Task<int> SaveChangesAsync();
  }
}