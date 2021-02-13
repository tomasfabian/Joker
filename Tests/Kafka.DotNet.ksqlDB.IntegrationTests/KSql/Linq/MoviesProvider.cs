using System;
using System.Threading.Tasks;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.IntegrationTests.KSql.RestApi;
using Kafka.DotNet.ksqlDB.IntegrationTests.Models.Movies;

namespace Kafka.DotNet.ksqlDB.IntegrationTests.KSql.Linq
{
  public class MoviesProvider
  {
    private readonly KSqlDbRestApiProvider restApiProvider;

    public MoviesProvider(KSqlDbRestApiProvider restApiProvider)
    {
      this.restApiProvider = restApiProvider ?? throw new ArgumentNullException(nameof(restApiProvider));
    }

    private string moviesTableName = "movies";
    private string actorsTableName = "lead_actor";

    public async Task<bool> CreateTablesAsync()
    {
      var createMoviesTable = $@"CREATE TABLE {moviesTableName} (
        title VARCHAR PRIMARY KEY,
        id INT,
        release_year INT
      ) WITH (
        KAFKA_TOPIC='{moviesTableName}',
        PARTITIONS=1,
        VALUE_FORMAT = 'JSON'
      );";

      var result = await restApiProvider.ExecuteStatementAsync(createMoviesTable);
      result.Should().BeTrue();

      var createActorsTable = $@"CREATE TABLE {actorsTableName} (
        title VARCHAR PRIMARY KEY,
        actor_name VARCHAR
      ) WITH (
        KAFKA_TOPIC='{actorsTableName}',
        PARTITIONS=1,
        VALUE_FORMAT='JSON'
      );";

      result = await restApiProvider.ExecuteStatementAsync(createActorsTable);
      result.Should().BeTrue();

      return true;
    }

    public static readonly Movie Movie1 = new()
    {
      Id = 1,
      Release_Year = 1986,
      Title = "Aliens"
    };

    public static readonly Lead_Actor LeadActor1 = new()
    {
      Actor_Name = "Sigourney Weaver",
      Title = "Aliens"
    };

    public async Task<bool> InsertMovieAsync(Movie movie)
    {
      string insert =
        $"INSERT INTO {moviesTableName} ({nameof(Movie.Id)}, {nameof(Movie.Title)}, {nameof(Movie.Release_Year)}) VALUES ({movie.Id}, '{movie.Title}', {movie.Release_Year});";

      var result = await restApiProvider.ExecuteStatementAsync(insert);      
      result.Should().BeTrue();

      return result;
    }

    public async Task<bool> InsertLeadAsync(Lead_Actor actor)
    {
      string insert =
        $"INSERT INTO {actorsTableName} ({nameof(Lead_Actor.Title)}, {nameof(Lead_Actor.Actor_Name)}) VALUES ('{actor.Title}', '{actor.Actor_Name}');";

      var result = await restApiProvider.ExecuteStatementAsync(insert);      
      result.Should().BeTrue();

      return result;
    }

    public async Task DropTablesAsync()
    {
      await restApiProvider.DropTableAndTopic(actorsTableName);
      await restApiProvider.DropTableAndTopic(moviesTableName);
    }
  }
}