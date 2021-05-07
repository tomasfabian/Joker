using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Statements;

namespace Kafka.DotNet.ksqlDB.KSql.RestApi
{
  public interface IKSqlDbRestApiClient
  {
    Task<HttpResponseMessage> ExecuteStatementAsync(KSqlDbStatement ksqlStatement, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> CreateTable<T>(EntityCreationMetadata creationMetadata, bool ifNotExists = false, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> CreateStream<T>(EntityCreationMetadata creationMetadata, bool ifNotExists = false, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> CreateOrReplaceTable<T>(EntityCreationMetadata creationMetadata, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> CreateOrReplaceStream<T>(EntityCreationMetadata creationMetadata, CancellationToken cancellationToken = default);
  }
}