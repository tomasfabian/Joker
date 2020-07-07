using Joker.Notifications;
using Joker.PubSubUI.Shared.ViewModels.Products;
using Joker.Redis.Notifications;
using Joker.WinUI3.Sample.Views;
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

      var productsEntityChangesViewModel = App.Kernel.Get<ProductsEntityChangesViewModel>();

      IDomainEntitiesSubscriber domainEntitiesSubscriber = App.Kernel.Get<IDomainEntitiesSubscriber>();
      domainEntitiesSubscriber.Subscribe();

      ProductsView.ViewModel = productsEntityChangesViewModel;
    }
  }
}