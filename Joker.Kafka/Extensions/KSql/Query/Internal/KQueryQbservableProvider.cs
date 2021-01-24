using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query
{
  internal class KQueryQbservableProvider : IKSqlQbservableProvider
  {
    private readonly IServiceProvider serviceProvider;

    public KQueryQbservableProvider(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    IQbservable<TResult> IQbservableProvider.CreateQuery<TResult>(Expression expression)
    {      
      var dependencies = serviceProvider.GetService<IKStreamSetDependencies>();

      return new KQuerySet<TResult>(dependencies, expression);
    }
  }
}