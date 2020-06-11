using System.Windows;
using Joker.PubSubUI.Shared.Navigation;

namespace Joker.WPF.Sample.Navigation
{
  public class DialogManager : IDialogManager
  {
    public void ShowMessage(string message)
    {
      MessageBox.Show(message);
    }
  }
}