using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Joker.PubSubUI.Shared.ViewModels.Products;
using Joker.Redis.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Ninject;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Joker.WinUI3.UWP.Sample
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();

      var productsEntityChangesViewModel = App.Kernel.Get<ProductsEntityChangesViewModel>();

      IDomainEntitiesSubscriber domainEntitiesSubscriber = App.Kernel.Get<IDomainEntitiesSubscriber>();
      domainEntitiesSubscriber.Subscribe();

      ProductsView.ViewModel = productsEntityChangesViewModel;
    }
  }
}