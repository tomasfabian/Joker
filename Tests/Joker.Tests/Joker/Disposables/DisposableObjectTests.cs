using FluentAssertions;
using Joker.Disposables;
using Joker.Extensions.Disposables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Joker.MVVM.Tests.Joker.Disposables
{
  [TestClass]
  public class DisposableObjectTests : TestBase<DisposableObject>
  {
    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();

      ClassUnderTest = new DisposableObject();
    }

    [TestMethod]
    public void Dispose_IsDisposed()
    {
      //Arrange

      //Act
      ClassUnderTest.Dispose();

      //Assert
      ClassUnderTest.IsDisposed.Should().BeTrue();
    }

    [TestMethod]
    public void Create_Dispose_IsDisposed()
    {
      //Arrange
      bool wasDisposed = false;
      var disposable = DisposableObject.Create(() => wasDisposed = true);

      //Act
      disposable.Dispose();

      //Assert
      wasDisposed.Should().BeTrue();
    }

    [TestMethod]
    public void DisposeComposite()
    {
      //Arrange
      var disposable = new DisposableObject();
      disposable.DisposeWith(ClassUnderTest.CompositeDisposable);

      //Act
      ClassUnderTest.Dispose();

      //Assert
      disposable.IsDisposed.Should().BeTrue();
    }
  }
}