using System.Data;

namespace Joker.Contracts.Data
{
  public interface IDbTransactionFactory
  {
    IDbTransaction BeginTransaction(IsolationLevel isolationLevel);
  }
}