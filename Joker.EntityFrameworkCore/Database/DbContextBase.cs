using System.Data;
using System.Data.Common;
using Joker.Contracts.Data;
using Microsoft.EntityFrameworkCore;

namespace Joker.EntityFrameworkCore.Database
{
  public class DbContextBase : DbContext, IContext, IDbTransactionFactory
  {
    #region Constructors


    #endregion

    public Joker.Contracts.Data.IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
    {
      var dbContextTransaction = Database.BeginTransaction(isolationLevel);

      return new DbTransaction(dbContextTransaction);
    }
  }
}