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

    public void Commit()
    {
      dbContextTransaction.Commit();
    }

    public void Rollback()
    {
      dbContextTransaction.Rollback();
    }
  }
}