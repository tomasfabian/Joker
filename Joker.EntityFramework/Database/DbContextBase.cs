using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using Joker.Contracts.Data;
using IDbTransaction = Joker.Contracts.Data.IDbTransaction;

namespace Joker.EntityFramework.Database
{
  public class DbContextBase : DbContext, IContext, IDbTransactionFactory
  {
    #region Constructors

    /// <summary>
    /// Constructs a new context instance using conventions to create the name of the database to
    /// which a connection will be made.  The by-convention name is the full name (namespace + class name)
    /// of the derived context class.
    /// See the class remarks for how this is used to create a connection.
    /// </summary>
    protected DbContextBase()
      : base()
    {
    }

    /// <summary>
    /// Constructs a new context instance using the given string as the name or connection string for the
    /// database to which a connection will be made.
    /// See the class remarks for how this is used to create a connection.
    /// </summary>
    /// <param name="nameOrConnectionString"> Either the database name or a connection string. </param>
    public DbContextBase(string nameOrConnectionString)
      : base(nameOrConnectionString)
    {
    }

    /// <summary>
    /// Constructs a new context instance using the given string as the name or connection string for the
    /// database to which a connection will be made, and initializes it from the given model.
    /// See the class remarks for how this is used to create a connection.
    /// </summary>
    /// <param name="nameOrConnectionString"> Either the database name or a connection string. </param>
    /// <param name="model"> The model that will back this context. </param>
    public DbContextBase(string nameOrConnectionString, DbCompiledModel model)
      : base(nameOrConnectionString, model)
    {
    }

    /// <summary>
    /// Constructs a new context instance using the existing connection to connect to a database.
    /// The connection will not be disposed when the context is disposed if <paramref name="contextOwnsConnection" />
    /// is <c>false</c>.
    /// </summary>
    /// <param name="existingConnection"> An existing connection to use for the new context. </param>
    /// <param name="contextOwnsConnection">
    /// If set to <c>true</c> the connection is disposed when the context is disposed, otherwise the caller must dispose the connection.
    /// </param>
    public DbContextBase(DbConnection existingConnection, bool contextOwnsConnection)
      : base(existingConnection, contextOwnsConnection)
    {
    }

    /// <summary>
    /// Constructs a new context instance using the existing connection to connect to a database,
    /// and initializes it from the given model.
    /// The connection will not be disposed when the context is disposed if <paramref name="contextOwnsConnection" />
    /// is <c>false</c>.
    /// </summary>
    /// <param name="existingConnection"> An existing connection to use for the new context. </param>
    /// <param name="model"> The model that will back this context. </param>
    /// <param name="contextOwnsConnection">
    ///     If set to <c>true</c> the connection is disposed when the context is disposed, otherwise the caller must dispose the connection.
    /// </param>
    public DbContextBase(
      DbConnection existingConnection,
      DbCompiledModel model,
      bool contextOwnsConnection)
      : base(existingConnection, model, contextOwnsConnection)
    {
    }

    /// <summary>
    /// Constructs a new context instance around an existing ObjectContext.
    /// </summary>
    /// <param name="objectContext"> An existing ObjectContext to wrap with the new context. </param>
    /// <param name="dbContextOwnsObjectContext">
    ///     If set to <c>true</c> the ObjectContext is disposed when the DbContext is disposed, otherwise the caller must dispose the connection.
    /// </param>
    public DbContextBase(ObjectContext objectContext, bool dbContextOwnsObjectContext)
      : base(objectContext, dbContextOwnsObjectContext)
    {
    }

    #endregion

    #region Methods

    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
    {
      var dbContextTransaction = Database.BeginTransaction(isolationLevel);

      return new DbTransaction(dbContextTransaction);
    }

    #endregion
  }
}