﻿using System;
using System.Threading.Tasks;
using Kafka.DotNet.ksqlDB.IntegrationTests.KSql.RestApi;
using Kafka.DotNet.ksqlDB.IntegrationTests.Models.Movies;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kafka.DotNet.ksqlDB.IntegrationTests.KSql.Linq
{
  [TestClass]
  public class JoinsTests : IntegrationTests
  {
    private static MoviesProvider moviesProvider;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
      RestApiProvider = KSqlDbRestApiProvider.Create();
      
      moviesProvider = new MoviesProvider(RestApiProvider);
      
      await moviesProvider.DropTablesAsync();
      
      await Task.Delay(TimeSpan.FromSeconds(1));

      await moviesProvider.CreateTablesAsync();

      await moviesProvider.InsertMovieAsync(MoviesProvider.Movie1);
      await moviesProvider.InsertLeadAsync(MoviesProvider.LeadActor1);
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
      await moviesProvider.DropTablesAsync();

      moviesProvider = null;
    }

    private string MoviesTableName => MoviesProvider.MoviesTableName;
    private string ActorsTableName => MoviesProvider.ActorsTableName;

    [TestMethod]
    public async Task Join()
    {
      //Arrange
      int expectedItemsCount = 1;

      var source = Context.CreateQueryStream<Movie>(MoviesTableName)
        .Join(
          Source.Of<Lead_Actor>(ActorsTableName),
          movie => movie.Title,
          actor => actor.Title,
          (movie, actor) => new
          {
            movie.Id,
            Title = movie.Title,
            movie.Release_Year,
            ActorTitle = actor.Title,
            Substr = K.Functions.Substring(actor.Title, 2, 4)
          }
        )
        .ToAsyncEnumerable();

      //Act
      var actualValues = await CollectActualValues(source, expectedItemsCount);

      //Assert
      Assert.AreEqual(expectedItemsCount, actualValues.Count);

      Assert.AreEqual(MoviesProvider.Movie1.Title, actualValues[0].Title);
      Assert.AreEqual(MoviesProvider.LeadActor1.Title, actualValues[0].Title);
      Assert.AreEqual("lien", actualValues[0].Substr);
      Assert.AreEqual(MoviesProvider.Movie1.Release_Year, actualValues[0].Release_Year);
    }

    [TestMethod]
    public async Task LeftJoin()
    {
      //Arrange
      int expectedItemsCount = 2;

      var source = Context.CreateQueryStream<Movie>(MoviesTableName)
        .LeftJoin(
          Source.Of<Lead_Actor>(ActorsTableName),
          movie => movie.Title,
          actor => actor.Title,
          (movie, actor) => new
          {
            movie.Id,
            Title = movie.Title,
            movie.Release_Year,
            ActorTitle = actor.Title,
            Substr = K.Functions.Substring(actor.Title, 2, 4)
          }
        )
        .ToAsyncEnumerable();

      //Act
      var actualValues = await CollectActualValues(source, expectedItemsCount);

      //Assert
      Assert.AreEqual(expectedItemsCount, actualValues.Count);

      Assert.AreEqual(MoviesProvider.Movie1.Title, actualValues[0].Title);
      Assert.AreEqual(MoviesProvider.LeadActor1.Title, actualValues[0].Title);
      Assert.IsNull(actualValues[0].ActorTitle);
      Assert.IsNull(actualValues[0].Substr);

      Assert.AreEqual("lien", actualValues[1].Substr);
      Assert.AreEqual(MoviesProvider.Movie1.Release_Year, actualValues[1].Release_Year);
      Assert.AreEqual(MoviesProvider.LeadActor1.Title, actualValues[1].ActorTitle);
    }
  }
}