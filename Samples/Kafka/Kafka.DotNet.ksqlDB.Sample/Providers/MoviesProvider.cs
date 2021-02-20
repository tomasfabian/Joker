using System;
using System.Net.Http;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.Sample.Models.Movies;

namespace Kafka.DotNet.ksqlDB.Sample.Providers
{
  public class MoviesProvider
  {
    private readonly IKSqlDbRestApiProvider restApiProvider;

    public MoviesProvider(IKSqlDbRestApiProvider restApiProvider)
    {
      this.restApiProvider = restApiProvider ?? throw new ArgumentNullException(nameof(restApiProvider));
    }

    public static readonly string MoviesTableName = "movies";
    public static readonly string ActorsTableName = "lead_actor";

    public async Task<bool> CreateTablesAsync()
    {
      var createMoviesTable = $@"CREATE TABLE {MoviesTableName} (
        title VARCHAR PRIMARY KEY,
        id INT,
        release_year INT
      ) WITH (
        KAFKA_TOPIC='{MoviesTableName}',
        PARTITIONS=1,
        VALUE_FORMAT = 'JSON'
      );";

      var result = await restApiProvider.ExecuteStatementAsync(createMoviesTable);

      var createActorsTable = $@"CREATE TABLE {ActorsTableName} (
        title VARCHAR PRIMARY KEY,
        actor_name VARCHAR
      ) WITH (
        KAFKA_TOPIC='{ActorsTableName}',
        PARTITIONS=1,
        VALUE_FORMAT='JSON'
      );";

      result = await restApiProvider.ExecuteStatementAsync(createActorsTable);

      return true;
    }

    public static readonly Movie Movie1 = new Movie()
    {
      Id = 1,
      Release_Year = 1986,
      Title = "Aliens"
    };

    public static readonly Movie Movie2 = new Movie()
    {
      Id = 2,
      Release_Year = 1998,
      Title = "Die Hard"
    };

    public static readonly Lead_Actor LeadActor1 = new Lead_Actor()
    {
      Actor_Name = "Sigourney Weaver",
      Title = "Aliens"
    };

    public async Task<HttpResponseMessage> InsertMovieAsync(Movie movie)
    {
      string insert =
        $"INSERT INTO {MoviesTableName} ({nameof(Movie.Id)}, {nameof(Movie.Title)}, {nameof(Movie.Release_Year)}) VALUES ({movie.Id}, '{movie.Title}', {movie.Release_Year});";

      var result = await restApiProvider.ExecuteStatementAsync(insert);      

      return result;
    }

    public async Task<HttpResponseMessage> InsertLeadAsync(Lead_Actor actor)
    {
      string insert =
        $"INSERT INTO {ActorsTableName} ({nameof(Lead_Actor.Title)}, {nameof(Lead_Actor.Actor_Name)}) VALUES ('{actor.Title}', '{actor.Actor_Name}');";

      var result = await restApiProvider.ExecuteStatementAsync(insert);      

      return result;
    }

    public async Task DropTablesAsync()
    {
      await restApiProvider.DropTableAndTopic(ActorsTableName);
      await restApiProvider.DropTableAndTopic(MoviesTableName);
    }
  }
}