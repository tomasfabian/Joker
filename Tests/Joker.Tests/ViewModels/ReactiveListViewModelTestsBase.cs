using System;
using Joker.Enums;
using Joker.MVVM.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;

namespace Joker.MVVM.Tests.ViewModels
{
  [TestClass]
  public abstract class ReactiveListViewModelTestsBase : TestBase<ReactiveListTestViewModel>
  {
    #region TestInitialize
    
    protected readonly TestModelProvider TestModelProvider = new TestModelProvider();
    protected readonly TestModelReactiveData ReactiveData = new TestModelReactiveData();

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();
    }
    
    protected readonly TestModel OriginalModel = new TestModel
    {
      Id = 2,
      Timestamp = new DateTime(2020, 3, 27, 16, 0, 0),
      Name = "Original"
    };

    #endregion

    #region Methods

    protected ReactiveListTestViewModel CreateReactiveListTestViewModel()
    {
      return new ReactiveListTestViewModel(ReactiveData, SchedulersFactory);
    }

    protected void SubscribeToDataChanges()
    {
      ClassUnderTest.SubscribeToDataChanges();

      LoadEntitiesFromQuery();
    }

    protected void LoadEntitiesFromQuery()
    {
      SchedulersFactory.ThreadPool.AdvanceBy(100);
      SchedulersFactory.Dispatcher.AdvanceBy(100);
    }

    protected readonly TimeSpan DefaultBufferTimeSpan = TimeSpan.FromMilliseconds(250);

    protected void AdvanceChangesBuffer(long bufferTicks)
    {
      SchedulersFactory.TaskPool.AdvanceBy(bufferTicks);
      SchedulersFactory.Dispatcher.AdvanceBy(bufferTicks);
    }    
    
    protected void AdvanceChangesBuffer()
    {
      AdvanceChangesBuffer(DefaultBufferTimeSpan.Ticks);
    }

    protected void SchedulePublishChangeEvent(ChangeType changeType, TestModel model, long ticks)
    {
      SchedulePublishChangeEvent(changeType, model, TimeSpan.FromTicks(ticks));
    }

    protected void SchedulePublishChangeEvent(ChangeType changeType, TestModel model, TimeSpan timeSpan)
    {
      ScheduleOnTaskPool(timeSpan, () => { PublishChangeEvent(changeType, model); });
    }

    private void PublishChangeEvent(ChangeType changeType, TestModel model)
    {
      ReactiveData.Publish(new EntityChange<TestModel>(model, changeType));
    }

    protected void PublishChangeEventAndAdvanceTime(ChangeType changeType, TestModel model)
    {
      PublishChangeEvent(changeType, model);

      AdvanceChangesBuffer();
    }

    protected void PublishEntityAddedEvent()
    {
      PublishChangeEventAndAdvanceTime(ChangeType.Create, OriginalModel);
    }

    protected void PublishEntityUpdatedEvent(TestModel model = null)
    {
      if(model == null)
      {
        model = OriginalModel.Clone();

        model.Name = "Renamed";
        model.Timestamp = OriginalModel.Timestamp.AddDays(1);
      }
      
      PublishChangeEventAndAdvanceTime(ChangeType.Update, model);
    }

    protected void PublishEntityDeletedEvent()
    {
      var clone = OriginalModel.Clone();

      clone.Timestamp = OriginalModel.Timestamp.AddMonths(1);

      PublishChangeEventAndAdvanceTime(ChangeType.Delete, clone);
    }

    #endregion
  }
}