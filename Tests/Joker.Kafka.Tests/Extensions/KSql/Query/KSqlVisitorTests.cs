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
    
      [TestMethod]
      public void BinaryEqual_BuildKSql_PrintsEqual()
      {
        //Arrange
        ConstantExpression constant = Expression.Constant(1);
        var expression = Expression.Equal(constant, constant);

        //Act
        var query = ClassUnderTest.BuildKSql(expression);

        //Assert
        query.Should().BeEquivalentTo("1 = 1");
      }
    
      [TestMethod]
      public void BinaryNotEqual_BuildKSql_PrintsNotEqual()
      {
        //Arrange
        ConstantExpression constant1 = Expression.Constant(1);
        ConstantExpression constant2 = Expression.Constant(2);
        var expression = Expression.NotEqual(constant1, constant2);

        //Act
        var query = ClassUnderTest.BuildKSql(expression);

        //Assert
        query.Should().BeEquivalentTo("1 != 2");
      }
    
      [TestMethod]
      public void BinaryLessThan_BuildKSql_PrintsLessThan()
      {
        //Arrange
        ConstantExpression constant1 = Expression.Constant(1);
        ConstantExpression constant2 = Expression.Constant(2);
        var expression = Expression.LessThan(constant1, constant2);

        //Act
        var query = ClassUnderTest.BuildKSql(expression);

        //Assert
        query.Should().BeEquivalentTo("1 < 2");
      }
          
      [TestMethod]
      public void BinaryLessThanOrEqual_BuildKSql_PrintsLessThanOrEqual()
      {
        //Arrange
        ConstantExpression constant1 = Expression.Constant(1);
        ConstantExpression constant2 = Expression.Constant(2);
        var expression = Expression.LessThanOrEqual(constant1, constant2);

        //Act
        var query = ClassUnderTest.BuildKSql(expression);

        //Assert
        query.Should().BeEquivalentTo("1 <= 2");
      }
          
      [TestMethod]
      public void BinaryGreaterThan_BuildKSql_PrintsGreaterThan()
      {
        //Arrange
        ConstantExpression constant1 = Expression.Constant(2);
        ConstantExpression constant2 = Expression.Constant(1);
        var expression = Expression.GreaterThan(constant1, constant2);

        //Act
        var query = ClassUnderTest.BuildKSql(expression);

        //Assert
        query.Should().BeEquivalentTo("2 > 1");
      }
          
      [TestMethod]
      public void BinaryGreaterThanOrEqual_BuildKSql_PrintsGreaterThanOrEqual()
      {
        //Arrange
        ConstantExpression constant1 = Expression.Constant(2);
        ConstantExpression constant2 = Expression.Constant(1);
        var expression = Expression.GreaterThanOrEqual(constant1, constant2);

        //Act
        var query = ClassUnderTest.BuildKSql(expression);

        //Assert
        query.Should().BeEquivalentTo("2 >= 1");
      }

      #endregion
  }
}