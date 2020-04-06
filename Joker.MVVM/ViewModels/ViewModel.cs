using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Joker.Contracts;

namespace Joker.MVVM.ViewModels
{
  public abstract class ViewModel<TModel> : ViewModel, IViewModel<TModel>
    where TModel : IVersion
  {
    protected ViewModel(TModel model)
    {
      if(model == null)
        throw new ArgumentNullException(nameof(model));

      Model = model;
    }

    public TModel Model { get; }
  }

  public abstract class ViewModel : IViewModel
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T backingProperty, 
      T value,
      [CallerMemberName]string propertyName = "",
      Action<T, T> onChanged = null)
    {
      if (EqualityComparer<T>.Default.Equals(backingProperty, value))
        return false;

      var oldValue = backingProperty;

      backingProperty = value;
      
      onChanged?.Invoke(oldValue, value);

      NotifyPropertyChanged(propertyName);

      return true;
    }
  }
}