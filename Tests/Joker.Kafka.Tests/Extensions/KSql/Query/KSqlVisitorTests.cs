using System.Linq.Expressions;
using FluentAssertions;
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

      #region Constants

      [TestMethod]
      public void NullConstant_BuildKSql_PrintsStringifiedNull()
      {
        //Arrange
        var constantExpression = Expression.Constant(null);

        //Act
        var query = ClassUnderTest.BuildKSql(constantExpression);

        //Assert
        query.Should().BeEquivalentTo("NULL");
      }

      [TestMethod]
      public void TextConstant_BuildKSql_PrintsTextSurroundedWithTicks()
      {
        //Arrange
        var constant = "TeSt Me";
        var constantExpression = Expression.Constant(constant);

        //Act
        var query = ClassUnderTest.BuildKSql(constantExpression);

        //Assert
        query.Should().BeEquivalentTo($"'{constant}'");
      }

      [TestMethod]
      public void ValueTypeConstant_BuildKSql_PrintsPlainText()
      {
        //Arrange
        var constant = 42;
        var constantExpression = Expression.Constant(constant);

        //Act
        var query = ClassUnderTest.BuildKSql(constantExpression);

        //Assert
        query.Should().BeEquivalentTo(constant.ToString());
      }

      [TestMethod]
      public void EnumerableConstant_BuildKSql_PrintsCommaSeparatedTextFields()
      {
        //Arrange
        var constant = new[] { "Field1", "Field2" };
        var constantExpression = Expression.Constant(constant);

        //Act
        var query = ClassUnderTest.BuildKSql(constantExpression);

        //Assert
        query.Should().BeEquivalentTo("Field1, Field2");
      }

      [TestMethod]
      public void ReferenceTypeConstant_BuildKSql_PrintsCommaSeparatedTextFields()
      {
        //Arrange
        var constant = new object();
        var constantExpression = Expression.Constant(constant);

        //Act
        var query = ClassUnderTest.BuildKSql(constantExpression);

        //Assert
        query.Should().BeEquivalentTo(constant.ToString());
      }

      #endregion

      #region Binary

      [TestMethod]
      public void BinaryAnd_BuildKSql_PrintsOperatorAnd()
      {
        //Arrange
        var andAlso = Expression.AndAlso(Expression.Constant(true), Expression.Constant(true));

        //Act
        var query = ClassUnderTest.BuildKSql(andAlso);

        //Assert
        query.Should().BeEquivalentTo("True AND True");
      }

      [TestMethod]
      public void BinaryOr_BuildKSql_PrintsOperatorOr()
      {
        //Arrange
        var orElse = Expression.OrElse(Expression.Constant(true), Expression.Constant(false));

        //Act
        var query = ClassUnderTest.BuildKSql(orElse);

        //Assert
        query.Should().BeEquivalentTo("True OR False");
      }

      #endregion
  }
}