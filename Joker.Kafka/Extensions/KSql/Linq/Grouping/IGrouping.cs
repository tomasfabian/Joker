namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Linq.Grouping
{
  public interface IGrouping<out TKey, out TElement> : IQbservable<TElement>
  {
    /// <summary>Gets the key of the <see cref="T:System.Linq.IGrouping`2" />.</summary>
    /// <returns>The key of the <see cref="T:System.Linq.IGrouping`2" />.</returns>
    TKey Key { get; }
  }
}