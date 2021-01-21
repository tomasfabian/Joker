using System.Net.Http;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi
{
  public interface IHttpClientFactory
  {
    HttpClient CreateClient();
  }
}