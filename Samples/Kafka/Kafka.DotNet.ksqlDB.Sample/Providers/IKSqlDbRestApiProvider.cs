using System.Net.Http;
using System.Threading.Tasks;

namespace Kafka.DotNet.ksqlDB.Sample.Providers
{
  public interface IKSqlDbRestApiProvider
  {
    Task<HttpResponseMessage> ExecuteStatementAsync(string ksql);
    Task<HttpResponseMessage> DropStreamAndTopic(string streamName);
    Task<HttpResponseMessage> DropTableAndTopic(string tableName);
  }
}