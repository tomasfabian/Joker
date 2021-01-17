using System.Linq.Expressions;
using Joker.Kafka.Extensions.ksql.Linq;

namespace Joker.Kafka.Extensions.KSql.Query
{
  public class QbservableProvider : IQbservableProvider
  {
    IQbservable<TResult> IQbservableProvider.CreateQuery<TResult>(Expression expression)
    {
      return new KStreamSet<TResult>(this, expression);
    }
  }
}