using System.Data;
using Joker.Contracts.Data;
using Microsoft.EntityFrameworkCore;
using IDbTransaction = Joker.Contracts.Data.IDbTransaction;

namespace Joker.EntityFrameworkCore.Database
{
  public class DbContextBase : DbContext, IContext, IDbTransactionFactory
  {
    #region Constructors

    /// <summary>
    ///     <para>
    ///         Initializes a new instance of the <see cref="DbContextBase" /> class. The
    ///         <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)" />
    ///         method will be called to configure the database (and other options) to be used for this context.
    ///     </para>
    /// </summary>
    protected DbContextBase()
    {
    }

    /// <summary>
    ///     <para>
    ///         Initializes a new instance of the <see cref="DbContextBase" /> class using the specified options.
    ///         The <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)" /> method will still be called to allow further
    ///         configuration of the options.
    ///     </para>
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public DbContextBase(DbContextOptions options)
      : base(options)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Begins a transaction on the underlying store connection using the specified isolation level
    /// </summary>
    /// <param name="isolationLevel">The database isolation level with which the underlying store transaction will be created</param>
    /// <returns>
    /// a <see cref="T:Joker.Contracts.Data.IDbTransaction" /> object wrapping access to the underlying store's transaction object
    /// </returns>
    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
    {
      var dbContextTransaction = Database.BeginTransaction(isolationLevel);

      return new DbTransaction(dbContextTransaction);
    }

    #endregion
  }
}