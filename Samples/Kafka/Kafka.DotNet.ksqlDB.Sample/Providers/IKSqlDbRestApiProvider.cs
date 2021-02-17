using System.Threading.Tasks;

namespace Kafka.DotNet.ksqlDB.Sample.Providers
{
  public interface IKSqlDbRestApiProvider
  {
    static string KSqlDbUrl { get; }
    Task<bool> ExecuteStatementAsync(string ksql);
    Task<bool> DropStreamAndTopic(string streamName);
    Task<bool> DropTableAndTopic(string tableName);
  }
}