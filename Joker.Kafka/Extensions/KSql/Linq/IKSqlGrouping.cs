namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Linq
{
  public interface IKSqlGrouping<out TKey, out TElement> : IAggregations<TElement>
  {
    TKey Key { get; }
  }
}