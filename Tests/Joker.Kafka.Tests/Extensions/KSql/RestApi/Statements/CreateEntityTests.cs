using System.Collections.Generic;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Enums;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Statements;
using NUnit.Framework;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.RestApi.Statements
{
  public class CreateEntityTests
  {
    readonly EntityCreationMetadata creationMetadata = new EntityCreationMetadata()
    {
      KafkaTopic = nameof(MyMovies),
      Partitions = 1,
      Replicas = 1
    };

    private string expectedStatementTemplate = @"{0} MyMovies (
	Id INT {1}KEY,
	Title VARCHAR,
	Release_Year INT,
	NumberOfDays ARRAY<INT>,
	Dictionary MAP<VARCHAR, INT>,
	Dictionary2 MAP<VARCHAR, INT>
) WITH ( KAFKA_TOPIC='MyMovies', VALUE_FORMAT='Json', PARTITIONS='1', REPLICAS='1' );";

    [Test]
    public void Print_CreateStream()
    {
      //Arrange
      var statementContext = new StatementContext
      {
        CreationType = CreationType.Create,
        KSqlEntityType = KSqlEntityType.Stream
      };

      //Act
      string statement = new CreateEntity().Print<MyMovies>(statementContext, creationMetadata, null);

      //Assert
      statement.Should().Be(string.Format(expectedStatementTemplate, "CREATE STREAM", string.Empty));
    }

    [Test]
    public void Print_CreateStream_WithIfNotExists()
    {
      //Arrange
      var statementContext = new StatementContext
      {
        CreationType = CreationType.Create,
        KSqlEntityType = KSqlEntityType.Stream
      };

      //Act
      string statement = new CreateEntity().Print<MyMovies>(statementContext, creationMetadata, ifNotExists: true);

      //Assert
      statement.Should().Be(string.Format(expectedStatementTemplate, "CREATE STREAM IF NOT EXISTS", string.Empty));
    }

    [Test]
    public void Print_CreateOrReplaceStream()
    {
      //Arrange
      var statementContext = new StatementContext
      {
        CreationType = CreationType.CreateOrReplace,
        KSqlEntityType = KSqlEntityType.Stream
      };

      //Act
      string statement = new CreateEntity().Print<MyMovies>(statementContext, creationMetadata, null);

      //Assert
      statement.Should().Be(string.Format(expectedStatementTemplate, "CREATE OR REPLACE STREAM", string.Empty));
    }

    [Test]
    public void Print_CreateTable()
    {
      //Arrange
      var statementContext = new StatementContext
      {
        CreationType = CreationType.Create,
        KSqlEntityType = KSqlEntityType.Table
      };

      //Act
      string statement = new CreateEntity().Print<MyMovies>(statementContext, creationMetadata, null);

      //Assert
      statement.Should().Be(string.Format(expectedStatementTemplate, "CREATE TABLE", "PRIMARY "));
    }

    [Test]
    public void Print_CreateTable_WithIfNotExists()
    {
      //Arrange
      var statementContext = new StatementContext
      {
        CreationType = CreationType.Create,
        KSqlEntityType = KSqlEntityType.Table
      };

      //Act
      string statement = new CreateEntity().Print<MyMovies>(statementContext, creationMetadata, ifNotExists: true);

      //Assert
      statement.Should().Be(string.Format(expectedStatementTemplate, "CREATE TABLE IF NOT EXISTS", "PRIMARY "));
    }

    [Test]
    public void Print_CreateOrReplaceTable()
    {
      //Arrange
      var statementContext = new StatementContext
      {
        CreationType = CreationType.CreateOrReplace,
        KSqlEntityType = KSqlEntityType.Table
      };

      //Act
      string statement = new CreateEntity().Print<MyMovies>(statementContext, creationMetadata, null);

      //Assert
      statement.Should().Be(string.Format(expectedStatementTemplate, "CREATE OR REPLACE TABLE", "PRIMARY "));
    }

    internal class MyMovies
    {
      [Kafka.DotNet.ksqlDB.KSql.RestApi.Statements.Annotations.Key]
      public int Id { get; set; }

      public string Title { get; set; }

      public int Release_Year { get; set; }

      public int[] NumberOfDays { get; set; }

      public IDictionary<string, int> Dictionary { get; set; }
      public Dictionary<string, int> Dictionary2 { get; set; }

      public int DontFindMe;

      public int DontFindMe2 { get; }
    }
  }
}