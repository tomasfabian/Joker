using System.ComponentModel;
using Joker.Contracts;

namespace Joker.MVVM.ViewModels
{
  public interface IViewModel : INotifyPropertyChanged
  {
  }

  public interface IViewModel<out TModel> : IViewModel
    where TModel : IVersion
  {
    TModel Model { get; }
  }
}