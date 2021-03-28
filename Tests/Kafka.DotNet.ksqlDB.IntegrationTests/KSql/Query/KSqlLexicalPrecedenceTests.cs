﻿using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.IntegrationTests.KSql.Linq;
using Kafka.DotNet.ksqlDB.IntegrationTests.KSql.RestApi;
using Kafka.DotNet.ksqlDB.IntegrationTests.Models.Movies;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kafka.DotNet.ksqlDB.IntegrationTests.KSql.Query
{
  [TestClass]
  public class KSqlLexicalPrecedenceTests : Linq.IntegrationTests
  {
    protected static MoviesProvider MoviesProvider;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
      await InitializeDatabase();
    }

    protected static async Task InitializeDatabase()
    {
      RestApiProvider = KSqlDbRestApiProvider.Create();

      MoviesProvider = new MoviesProvider(RestApiProvider);
      await MoviesProvider.CreateTablesAsync();

      await MoviesProvider.InsertMovieAsync(MoviesProvider.Movie1);
      await MoviesProvider.InsertMovieAsync(MoviesProvider.Movie2);
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
      await MoviesProvider.DropTablesAsync();

      MoviesProvider = null;
    }

    protected string MoviesTableName => MoviesProvider.MoviesTableName;

    protected virtual IQbservable<Movie> MoviesStream => Context.CreateQueryStream<Movie>(MoviesTableName);

    [TestMethod]
    public async Task Select()
    {
      //Arrange
      int expectedItemsCount = 1;

      //Act
      var source = MoviesStream.Select(c => new
      {
        First = c.Id + 2 * 3,
        ChangedOrder = (c.Id + 2) * 3
      }).ToAsyncEnumerable();

      var actualValues = await CollectActualValues(source, expectedItemsCount);

      //Assert
      var actualArr = actualValues.First();

      actualArr.First.Should().Be(7);
      actualArr.ChangedOrder.Should().Be(9);
    }

    [TestMethod]
    public async Task Where()
    {
      //Arrange
      int expectedItemsCount = 1;

      //Act
      var source = MoviesStream.Where(c => (c.Title == "Aliens" || c.Title == "Die Hard") && c.Release_Year >= 1990).ToAsyncEnumerable();

      var actualValues = await CollectActualValues(source, expectedItemsCount);

      //Assert
      var movie = actualValues.First();
      
      movie.Id.Should().Be(MoviesProvider.Movie2.Id);
    }

    [TestMethod]
    public async Task Where_NoBrackets()
    {
      //Arrange
      int expectedItemsCount = 1;

      //Act
      var source = MoviesStream.Where(c => c.Title == "Aliens" || c.Title == "Die Hard" && c.Release_Year >= 1990).ToAsyncEnumerable();

      var actualValues = await CollectActualValues(source, expectedItemsCount);

      //Assert
      var movie = actualValues.First();

      movie.Id.Should().Be(MoviesProvider.Movie1.Id);
    }
  }
}