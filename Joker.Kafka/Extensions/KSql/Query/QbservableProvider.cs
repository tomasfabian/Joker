using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  public class QbservableProvider : IKSqlQbservableProvider
  {
    private readonly IServiceProvider serviceProvider;

    public QbservableProvider(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }
    
    IQbservable<TResult> IQbservableProvider.CreateQuery<TResult>(Expression expression)
    {
      var dependencies = serviceProvider.GetService<IKStreamSetDependencies>();

      return new KQueryStreamSet<TResult>(dependencies, expression);
    }
  }
}