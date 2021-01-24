using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Sample.Model;
using Pluralize.NET;

namespace Kafka.DotNet.ksqlDB.Sample.QueryStreams
{
  public class PeopleQueryStream : KQueryStreamSet<Person>
  {
    public PeopleQueryStream(string ksqlDbUrl)
      : base(new QbservableProvider(ksqlDbUrl))
    {
    }

    public override IKSqlQueryGenerator CreateKSqlQueryGenerator()
    {
      return new EnglishKSqlQueryGenerator();
    }

    private class EnglishKSqlQueryGenerator : KSqlQueryGenerator
    {
      //Install-Package Pluralize.NET -Version 1.0.2
      private static readonly IPluralize EnglishPluralizationService = new Pluralizer();

      protected override string InterceptStreamName(string value)
      {
        var baseStreamName = base.InterceptStreamName(value);//trivially adds s to the end of the value

        return EnglishPluralizationService.Pluralize(value);
      }
    }
  }
}