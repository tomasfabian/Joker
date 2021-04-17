using System;
using System.Text;

namespace Kafka.DotNet.ksqlDB.KSql.RestApi.Statements
{
  public sealed record KSqlDbStatement
  {
    public KSqlDbStatement(string statement)
    {
      if (string.IsNullOrEmpty(statement))
        throw new NullReferenceException(nameof(statement));

      StatementText = statement;
    }

    public string StatementText { get; }

    public EndpointType EndpointType { get; set; } = EndpointType.KSql;

    public Encoding ContentEncoding { get; set; } = Encoding.UTF8;

    public long? CommandSequenceNumber  { get; set; }
  }
}