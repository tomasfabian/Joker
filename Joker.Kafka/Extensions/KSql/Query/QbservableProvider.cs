using System;
using System.Linq.Expressions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public class QbservableProvider : IKSqlQbservableProvider
  {
    public QbservableProvider(string url)
    {
      if(string.IsNullOrEmpty(url))
        throw new ArgumentNullException(nameof(url));

      Url = url;
    }
    
    IQbservable<TResult> IQbservableProvider.CreateQuery<TResult>(Expression expression)
    {
      return new KQueryStreamSet<TResult>(this, expression);
    }

    public string Url { get; }
  }
}