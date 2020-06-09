using Joker.WPF.Sample.ViewModels.Reactive;
using Microsoft.UI.Xaml;
using Ninject;

// The Blank Window item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Joker.WinUI3.Sample
{
  /// <summary>
  /// An empty window that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      root.DataContext = App.Kernel.Get<ProductsEntityChangesViewModel>();
    }
  }
}