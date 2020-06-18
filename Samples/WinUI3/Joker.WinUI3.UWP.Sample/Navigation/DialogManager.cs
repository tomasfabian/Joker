using System;
using Joker.PubSubUI.Shared.Navigation;
using Microsoft.UI.Xaml.Controls;

namespace Joker.WinUI3.UWP.Sample.Navigation
{
  public class DialogManager : IDialogManager
  {
    public void ShowMessage(string message)
    {
      ContentDialog dialog = new ContentDialog()
      {
        Title = "Error",
        Content = message,
        CloseButtonText = "Ok"
      };

      dialog.ShowAsync();
    }
  }
}