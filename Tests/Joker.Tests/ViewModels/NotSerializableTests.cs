using System;
using System.Runtime.Serialization;
using Joker.Enums;
using Joker.MVVM.Tests.Helpers;
using Joker.Reactive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Joker.MVVM.Tests.ViewModels
{
  [TestClass]
  public class NotSerializableTests : TestBase<NotSerializableTestModelViewModels>
  {    
    #region TestInitialize

    protected readonly ReactiveData<NotSerializableTestModel> ReactiveData = new ReactiveData<NotSerializableTestModel>();

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();
      
      //Init();

      ClassUnderTest = CreateReactiveListTestViewModel();
      ClassUnderTest.SubscribeToDataChanges();

      LoadEntitiesFromQuery();
    }

    #endregion

    #region Methods    
    
    private void LoadEntitiesFromQuery()
    {
      SchedulersFactory.ThreadPool.AdvanceBy(100);
      SchedulersFactory.Dispatcher.AdvanceBy(100);
    }
    
    protected NotSerializableTestModelViewModels CreateReactiveListTestViewModel()
    {
      return new NotSerializableTestModelViewModels(ReactiveData, SchedulersFactory);
    }    
    
    protected readonly TimeSpan DefaultBufferTimeSpan = TimeSpan.FromMilliseconds(250);

    protected void AdvanceChangesBuffer(long bufferTicks)
    {
      SchedulersFactory.TaskPool.AdvanceBy(bufferTicks);
      SchedulersFactory.Dispatcher.AdvanceBy(bufferTicks);
    }   

    #endregion    
    
    #region Tests

    [TestMethod]
    [ExpectedException(typeof(SerializationException))]
    public void ThrowException()
    {
      //Arrange
      ReactiveData.Publish(new EntityChange<NotSerializableTestModel>(new NotSerializableTestModel { Id = 1 }, ChangeType.Create));

      //Act
      AdvanceChangesBuffer(DefaultBufferTimeSpan.Ticks);

      //Assert
    }

    #endregion    
  }
}