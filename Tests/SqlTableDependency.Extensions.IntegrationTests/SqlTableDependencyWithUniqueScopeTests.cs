using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Domain.Models;
using UnitTests;

namespace SqlTableDependency.Extensions.IntegrationTests
{
  [TestClass]
  public class SqlTableDependencyWithUniqueScopeTests : TestBase
  {
    private static string ConnectionString => "Server=127.0.0.1,1402;User Id = SA;Password=<YourNewStrong@Passw0rd>;Initial Catalog = Test;";

    private SqlTableDependencyWithUniqueScope<Product> SqlTableDependency { get; set; }

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      SqlTableDependency = CreateClassUnderTest();
    }
    
    [TestMethod]
    public void UniqueName_DefaultIsMachineName()
    {
      //Arrange

      //Act
      var uniqueName = SqlTableDependency.UniqueName;

      //Assert
      uniqueName.Should().Be(Environment.MachineName);
    }
    
    [TestMethod]
    public void UniqueName_SettingsEmptyName_DefaultIsMachineName()
    {
      //Arrange
      SqlTableDependency.Settings = new SqlTableDependencySettings<Product>
                                    {
                                      FarServiceUniqueName = string.Empty,
                                    };

      //Act
      var uniqueName = SqlTableDependency.UniqueName;

      //Assert
      uniqueName.Should().Be(Environment.MachineName);
    }
    
    [TestMethod]
    public void UniqueName_Settings_DefaultIsMachineName()
    {
      //Arrange
      var expectedUniqueName = "testName";

      SqlTableDependency.Settings = new SqlTableDependencySettings<Product>
      {
        FarServiceUniqueName = expectedUniqueName,
      };

      //Act
      var uniqueName = SqlTableDependency.UniqueName;

      //Assert
      uniqueName.Should().Be(expectedUniqueName);
    }

    private SqlTableDependencyWithUniqueScope<Product> CreateClassUnderTest()
    {
      return new SqlTableDependencyWithUniqueScope<Product>(ConnectionString, "Products");
    }
  }
}