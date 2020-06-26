using System;

namespace Joker.BlazorApp.Sample.Navigation
{
  public interface IBlazorDialogManager
  {
    IObservable<string> ErrorMessages { get; }
  }
}