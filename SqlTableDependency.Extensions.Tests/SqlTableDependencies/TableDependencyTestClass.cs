using System.Diagnostics;
using System.Globalization;
using System.Text;
using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.Delegates;
using TableDependency.SqlClient.Base.Enums;

namespace SqlTableDependency.Extensions.Tests.SqlTableDependencies
{
  internal class TableDependencyTestClass<TEntity> : TestBase, ITableDependency<TEntity>
    where TEntity : class, new()
  {
    public TraceLevel TraceLevel { get; set; }
    public TraceListener TraceListener { get; set; }
    public TableDependencyStatus Status { get; }
    public Encoding Encoding { get; set; }
    public CultureInfo CultureInfo { get; set; }
    public string DataBaseObjectsNamingConvention { get; protected set; }
    public string TableName { get; protected set; }
    public string SchemaName { get; protected set; }

    public event ErrorEventHandler OnError;
    public event StatusEventHandler OnStatusChanged;
    public event ChangedEventHandler<TEntity> OnChanged;

    public void Start(int timeOut = 120, int watchDogTimeOut = 180)
    {
    }

    public void Stop()
    {
    }

    public void Dispose()
    {
    }
  }
}