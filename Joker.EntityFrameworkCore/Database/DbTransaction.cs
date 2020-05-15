using System;
using Joker.Contracts.Data;
using Joker.Disposables;
using Microsoft.EntityFrameworkCore.Storage;

namespace Joker.EntityFrameworkCore.Database
{
  public class DbTransaction : DisposableObject, IDbTransaction
  {
    private readonly IDbContextTransaction dbContextTransaction;

    public DbTransaction(IDbContextTransaction  dbContextTransaction)
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