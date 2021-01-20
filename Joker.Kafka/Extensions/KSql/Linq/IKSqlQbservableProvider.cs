namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Linq
{
  public interface IKSqlQbservableProvider : IQbservableProvider
  {
    string Url { get; }
  }
}