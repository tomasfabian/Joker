using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Joker.PubSubUI.Shared.Navigation;

namespace Joker.BlazorApp.Sample.Navigation
{
  public class DialogManager : IDialogManager, IBlazorDialogManager
  {
    public void ShowMessage(string message)
    {
      errorMessageSubject.OnNext(message);
    }

    private readonly ISubject<string> errorMessageSubject = new Subject<string>();

    public IObservable<string> ErrorMessages => errorMessageSubject.AsObservable();
  }
}