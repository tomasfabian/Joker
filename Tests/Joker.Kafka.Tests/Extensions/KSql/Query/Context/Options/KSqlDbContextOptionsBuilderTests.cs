﻿using System;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.KSql.Query.Context.Options;
using Kafka.DotNet.ksqlDB.KSql.Query.Options;
using Kafka.DotNet.ksqlDB.KSql.RestApi.Parameters;
using Kafka.DotNet.ksqlDB.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;
using static System.String;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Query.Context.Options
{
  [TestClass]
  public class KSqlDbContextOptionsBuilderTests : TestBase<KSqlDbContextOptionsBuilder>
  {
    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = new KSqlDbContextOptionsBuilder();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void UseKSqlDb_NullUrl_ThrowsArgumentNullException()
    {
      //Arrange

      //Act
      var options = ClassUnderTest.UseKSqlDb(null).Options;

      //Assert
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void UseKSqlDb_EmptyStringUrl_ThrowsArgumentNullException()
    {
      //Arrange

      //Act
      var options = ClassUnderTest.UseKSqlDb(Empty).Options;

      //Assert
    }

    [TestMethod]
    public void UseKSqlDb_OptionsContainsFilledUrl()
    {
      //Arrange

      //Act
      var options = ClassUnderTest.UseKSqlDb(TestParameters.KsqlDBUrl).Options;

      //Assert
      options.Url.Should().BeEquivalentTo(TestParameters.KsqlDBUrl);
    }

    #region QueryStream

    [TestMethod]
    public void SetupQueryStream_OptionsQueryStreamParameters_AutoOffsetResetIsSetToDefault()
    {
      //Arrange
      var setupParameters = ClassUnderTest.UseKSqlDb(TestParameters.KsqlDBUrl);

      //Act
      var options = setupParameters.SetupQueryStream(c =>
      {

      }).Options;

      //Assert
      options.QueryStreamParameters.Properties[QueryStreamParameters.AutoOffsetResetPropertyName].Should().BeEquivalentTo("earliest");
    }

    [TestMethod]
    public void SetupQueryStreamNotCalled_OptionsQueryStreamParameters_AutoOffsetResetIsSetToDefault()
    {
      //Arrange
      var setupParameters = ClassUnderTest.UseKSqlDb(TestParameters.KsqlDBUrl);
      string earliestAtoOffsetReset = AutoOffsetReset.Earliest.ToString().ToLower();

      //Act
      var options = setupParameters.Options;

      //Assert
      options.QueryStreamParameters.Properties[QueryStreamParameters.AutoOffsetResetPropertyName].Should().BeEquivalentTo(earliestAtoOffsetReset);
    }

    [TestMethod]
    public void SetupQueryStream_AmendOptionsQueryStreamParametersProperty_AutoOffsetResetWasChanged()
    {
      //Arrange
      var setupParameters = ClassUnderTest.UseKSqlDb(TestParameters.KsqlDBUrl);
      string latestAtoOffsetReset = AutoOffsetReset.Latest.ToString().ToLower();

      //Act
      var options = setupParameters.SetupQueryStream(c =>
      {
        c.Properties[QueryStreamParameters.AutoOffsetResetPropertyName] = latestAtoOffsetReset;
      }).Options;

      //Assert
      options.QueryStreamParameters.Properties[QueryStreamParameters.AutoOffsetResetPropertyName].Should().BeEquivalentTo(latestAtoOffsetReset);
    }

    #endregion

    #region Query

    [TestMethod]
    public void SetupQuery_OptionsQueryParameters_AutoOffsetResetIsSetToDefault()
    {
      //Arrange
      var setupParameters = ClassUnderTest.UseKSqlDb(TestParameters.KsqlDBUrl);

      //Act
      var options = setupParameters.SetupQuery(c =>
      {

      }).Options;

      //Assert
      options.QueryParameters.Properties[QueryParameters.AutoOffsetResetPropertyName].Should().BeEquivalentTo("earliest");
    }

    [TestMethod]
    public void SetupQueryNotCalled_OptionsQueryParameters_AutoOffsetResetIsSetToDefault()
    {
      //Arrange
      var setupParameters = ClassUnderTest.UseKSqlDb(TestParameters.KsqlDBUrl);
      string earliestAtoOffsetReset = AutoOffsetReset.Earliest.ToString().ToLower();

      //Act
      var options = setupParameters.Options;

      //Assert
      options.QueryParameters.Properties[QueryParameters.AutoOffsetResetPropertyName].Should().BeEquivalentTo(earliestAtoOffsetReset);
    }

    [TestMethod]
    public void SetupQuery_AmendOptionsQueryParametersProperty_AutoOffsetResetWasChanged()
    {
      //Arrange
      var setupParameters = ClassUnderTest.UseKSqlDb(TestParameters.KsqlDBUrl);
      string latestAtoOffsetReset = AutoOffsetReset.Latest.ToString().ToLower();

      //Act
      var options = setupParameters.SetupQuery(c =>
      {
        c.Properties[QueryParameters.AutoOffsetResetPropertyName] = latestAtoOffsetReset;
      }).Options;

      //Assert
      options.QueryParameters.Properties[QueryParameters.AutoOffsetResetPropertyName].Should().BeEquivalentTo(latestAtoOffsetReset);
    }

    #endregion
  }
}