using System;
using Joker.PubSubUI.Shared.Navigation;
#if HAS_UNO
using Windows.UI.Xaml.Controls;
#elif WINDOWS || NETCOREAPP || NETFRAMEWORK
using System.Windows;      
#elif NETSTANDARD
#else
using Microsoft.UI.Xaml.Controls;
#endif

namespace Joker.PubSubUI.Shared.Navigation
{
  public class DialogManager : IDialogManager
  {
    public void ShowMessage(string message)
    {
#if WINDOWS || NETCOREAPP || NETFRAMEWORK
      MessageBox.Show(message);
#elif NETSTANDARD
        Console.WriteLine(message);
#else
      ContentDialog dialog = new ContentDialog()
      {
        Title = "Error",
        Content = message,
        CloseButtonText = "Ok"
      };

      dialog.ShowAsync();
#endif
        }
    }
}