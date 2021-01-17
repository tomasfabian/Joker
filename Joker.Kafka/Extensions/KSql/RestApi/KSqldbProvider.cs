using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi
{
  public class KSqldbProvider<T> : IKSqldbProvider<T>
  {
    private readonly HttpClient httpClient;

    public KSqldbProvider(HttpClient httpClient)
    {
      this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public KSqldbProvider(string url)
    {
      httpClient = new HttpClient
      {
        BaseAddress = new Uri(url)
      };
    }

    public async IAsyncEnumerable<T> Run([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
      //https://docs.ksqldb.io/en/latest/developer-guide/api/
      //TODO: connect to ksqldb REST API
      await Task.FromResult(default(T));
      
      while (true)
      {
        if(cancellationToken.IsCancellationRequested)
          yield break;

        yield return default;
      }
    }
  }
}