using System;
using System.Linq.Expressions;
using FluentAssertions;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;
using Location = Kafka.DotNet.ksqlDB.Tests.Models.Location;

namespace Kafka.DotNet.ksqlDB.Tests.Extensions.KSql.Query
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

      #region New

      [TestMethod]
      public void NewAnonymousType_BuildKSql_PrintsMemberName()
      {
        //Arrange
        Expression<Func<Location, object>> expression = l => new { l.Longitude };

        //Act
        var query = ClassUnderTest.BuildKSql(expression);

        //Assert
        query.Should().BeEquivalentTo(nameof(Location.Longitude));
      }

      [TestMethod]
      public void NewAnonymousTypeMultipleMembers_BuildKSql_PrintsAllCommaSeparatedMemberNames()
      {
        //Arrange
        Expression<Func<Location, object>> expression = l => new { l.Longitude, l.Latitude };

        //Act
        var query = ClassUnderTest.BuildKSql(expression);

        //Assert
        query.Should().BeEquivalentTo($"{nameof(Location.Longitude)}, {nameof(Location.Latitude)}");
      }

      [TestMethod]
      public void NewAnonymousTypeMultipleMembersOneHasAlias_BuildKSql_PrintsAllCommaSeparatedMemberNames()
      {
        //Arrange
        Expression<Func<Location, object>> expression = l => new { l.Longitude, La = l.Latitude };

        //Act
        var query = ClassUnderTest.BuildKSql(expression);

        //Assert
        query.Should().BeEquivalentTo($"{nameof(Location.Longitude)}, La");
      }

      [TestMethod]
      public void NewReferenceType_BuildKSql_PrintsNothing()
      {
        //Arrange
        Expression<Func<Location, object>> expression = l => new Location();

        //Act
        var query = ClassUnderTest.BuildKSql(expression);

        //Assert
        query.Should().BeEmpty();
      }

      [TestMethod]
      public void NewMemberInit_BuildKSql_PrintsNothing()
      {
        //Arrange
        Expression<Func<Location, object>> expression = l => new Location { Latitude = "t" };

        //Act
        var query = ClassUnderTest.BuildKSql(expression);

        //Assert
        query.Should().BeEmpty();
      }

      #endregion

      #region MemberAccess

      [TestMethod]
      public void MemberAccess_BuildKSql_PrintsNameOfTheProperty()
      {
        //Arrange
        Expression<Func<Location, double>> predicate = l => l.Longitude;

        //Act
        var query = ClassUnderTest.BuildKSql(predicate);

        //Assert
        query.Should().BeEquivalentTo(nameof(Location.Longitude));
      }

      [TestMethod]
      public void Predicate_BuildKSql_PrintsOperatorAndOperands()
      {
        //Arrange
        Expression<Func<Location, bool>> predicate = l => l.Latitude != "ahoj svet";

        //Act
        var query = ClassUnderTest.BuildKSql(predicate);

        //Assert
        query.Should().BeEquivalentTo($"{nameof(Location.Latitude)} != 'ahoj svet'");
      }

      [Ignore("Figure out what to do")]
      [TestMethod]
      public void PredicateCompareWithVariable_BuildKSql_PrintsOperatorAndOperands()
      {
        //Arrange
        string value = "ahoj svet";

        Expression<Func<Location, bool>> predicate = l => l.Latitude != value;

        //Act
        var query = ClassUnderTest.BuildKSql(predicate);

        //Assert
        query.Should().BeEquivalentTo($"{nameof(Location.Latitude)} != '${value}'");
      }

      [TestMethod]
      public void PredicateCompareWithDouble_BuildKSql_PrintsOperatorAndOperands()
      {
        //Arrange
        Expression<Func<Location, bool>> predicate = l => l.Longitude == 1.2;

        //Act
        var query = ClassUnderTest.BuildKSql(predicate);

        //Assert
        query.Should().BeEquivalentTo($"{nameof(Location.Longitude)} = 1.2");
      }

      #endregion

      #region Parameter

      [TestMethod]
      public void Parameter_BuildKSql_PrintsParameterName()
      {
        //Arrange
        var expression = Expression.Parameter(typeof(int), "param");

        //Act
        var ksql = ClassUnderTest.BuildKSql(expression);

        //Assert
        ksql.Should().BeEmpty();
      }

      #endregion
  }
}