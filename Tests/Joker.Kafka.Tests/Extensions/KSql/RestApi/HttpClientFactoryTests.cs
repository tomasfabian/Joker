using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.RestApi;
using Kafka.DotNet.ksqlDB.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.RestApi
{
  [TestClass]
  public class HttpClientFactoryTests : TestBase
  {
    [TestMethod]
    public async Task CreateClient_BaseAddressWasSet()
    {
      //Arrange
      var httpClientFactory = new HttpClientFactory(new Uri(TestParameters.KsqlDBUrl));

      //Act
      var httpClient = httpClientFactory.CreateClient();

      //Assert
      httpClient.Should().BeOfType<HttpClient>();
      httpClient.BaseAddress.OriginalString.Should().BeEquivalentTo(TestParameters.KsqlDBUrl);
    }
  }
}