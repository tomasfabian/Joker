using System;
using System.Linq.Expressions;

namespace Joker.Kafka.Extensions.ksql.Linq
{
  public interface IQbservableProvider
  {
    /// <summary>
    /// Constructs an <see cref="IQbservable{T}"/> object that can evaluate the query represented by a specified expression tree.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements of the <see cref="IQbservable{T}"/> that is returned.</typeparam>
    /// <param name="expression">Expression tree representing the query.</param>
    /// <returns>IQbservable object that can evaluate the given query expression.</returns>
    IQbservable<TResult> CreateQuery<TResult>(Expression expression);
  }
}