using System;
using System.Data.Entity;
using Joker.Contracts.Data;
using Joker.Disposables;

namespace Joker.EntityFramework.Database
{
  public class DbTransaction : DisposableObject, IDbTransaction
  {
    private readonly DbContextTransaction dbContextTransaction;

    public DbTransaction(DbContextTransaction dbContextTransaction)
    {
      this.dbContextTransaction = dbContextTransaction ?? throw new ArgumentNullException(nameof(dbContextTransaction));
    }

    /// <summary>
    ///     Commits all changes made to the database in the current transaction.
    /// </summary>
    public void Commit()
    {
      dbContextTransaction.Commit();
    }

    /// <summary>
    ///     Discards all changes made to the database in the current transaction.
    /// </summary>
    public void Rollback()
    {
      dbContextTransaction.Rollback();
    }
  }
}