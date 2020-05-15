using System;
using System.Threading;
using System.Threading.Tasks;
using Joker.Contracts.Data;
using Joker.Disposables;
using Microsoft.EntityFrameworkCore.Storage;

namespace Joker.EntityFrameworkCore.Database
{
  public class DbTransaction : DisposableObject, IDbTransaction, IAsyncDbTransaction
  {
    private readonly IDbContextTransaction dbContextTransaction;

    public DbTransaction(IDbContextTransaction dbContextTransaction)
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
    ///     Commits all changes made to the database in the current transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken"> The cancellation token. </param>
    /// <returns> A <see cref="Task" /> representing the asynchronous operation. </returns>
    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
      return dbContextTransaction.CommitAsync(cancellationToken);
    }

    /// <summary>
    ///     Discards all changes made to the database in the current transaction.
    /// </summary>
    public void Rollback()
    {
      dbContextTransaction.Rollback();
    }

    /// <summary>
    ///     Discards all changes made to the database in the current transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken"> The cancellation token. </param>
    /// <returns> A <see cref="Task" /> representing the asynchronous operation. </returns>
    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
      return dbContextTransaction.RollbackAsync(cancellationToken);
    }
  }
}