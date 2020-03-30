using Joker.MVVM.ViewModels.Domain;

namespace Joker.MVVM.Tests.Helpers
{
  public class NotSerializableTestModelViewModel : DomainEntityViewModel<NotSerializableTestModel>
  {
    public NotSerializableTestModelViewModel(NotSerializableTestModel model)
      : base(model)
    {
    }
  }
}