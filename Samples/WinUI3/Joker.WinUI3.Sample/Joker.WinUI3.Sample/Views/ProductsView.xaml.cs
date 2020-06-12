using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Joker.PubSubUI.Shared.ViewModels.Products;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Joker.WinUI3.Sample.Views
{
  public sealed partial class ProductsView : UserControl
  {
    public ProductsView()
    {
      InitializeComponent();
    }

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(ProductsEntityChangesViewModel), typeof(ProductsView), new PropertyMetadata(null));

    public ProductsEntityChangesViewModel ViewModel
    {
      get { return GetValue(ViewModelProperty) as ProductsEntityChangesViewModel; }
      set { SetValue(ViewModelProperty, value); }
    }
  }
}