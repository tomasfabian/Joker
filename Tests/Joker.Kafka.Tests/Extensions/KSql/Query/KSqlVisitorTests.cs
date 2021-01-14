using Joker.Kafka.Extensions.KSql.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Joker.Kafka.Tests.Extensions.KSql.Query
{
  [TestClass]
  public class KSqlVisitorTests : TestBase<KSqlVisitor>
  {
      [TestInitialize]
      public override void TestInitialize()
      {
        base.TestInitialize();

        ClassUnderTest = new KSqlVisitor();
      }
  }
}