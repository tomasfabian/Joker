using Joker.BlazorApp.Sample.Subscribers;
using Joker.Platforms.Factories.Schedulers;
using Joker.PubSubUI.Shared.ViewModels.Products;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using OData.Client;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Joker.Extensions.Disposables;

namespace Joker.BlazorApp.Sample.Pages
{
  public class ProductsPageComponentBase : ComponentBase, IDisposable
  {
    #region Injected Services

    [Inject]
    public ProductsEntityChangesViewModel ViewModel { get; set; }

    [Inject]
    public IPlatformSchedulersFactory PlatformSchedulersFactory { get; set; }

    [Inject]
    public IDomainEntitiesSubscriber DomainEntitiesSubscriber { get; set; }

    [Inject]
    public IODataServiceContextFactory DataServiceClientFactory { get; set; }

    #endregion

    #region SignalR

    private HubConnection hubConnection;

    public bool IsConnected =>
      hubConnection?.State == HubConnectionState.Connected;

    #endregion
    
    private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

    protected override async Task OnInitializedAsync()
    {
      Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ViewModel.PropertyChanged  += h,
          h => ViewModel.PropertyChanged  -= h)
        .Where(c => c.EventArgs.PropertyName == nameof(ProductsEntityChangesViewModel.IsOffline))
        .Subscribe(c =>
        {
          (ViewModel.ProductsListViewModel.Items as INotifyCollectionChanged).CollectionChanged -= ProductsPageComponentBase_CollectionChanged;
          (ViewModel.ProductsListViewModel.Items as INotifyCollectionChanged).CollectionChanged += ProductsPageComponentBase_CollectionChanged;
          StateHasChanged();
        })
        .DisposeWith(compositeDisposable);
      
      //ViewModel.ProductsListViewModel.SelectionChanged
      //  .Subscribe(c => StateHasChanged())
      //  .DisposeWith(compositeDisposable);

      await DomainEntitiesSubscriber.Subscribe();

      await base.OnInitializedAsync();
    }

    private void ProductsPageComponentBase_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      StateHasChanged();
    }

    public void UpdateProduct()
    {
      var selectedProduct = ViewModel.ProductsListViewModel?.SelectedItem;
      if (selectedProduct != null && selectedProduct.Update.CanExecute(null))
        selectedProduct.Update.Execute(null);

      //selectedProduct.Update.Execute(null);
    }

    public void Dispose()
    {
      compositeDisposable.Dispose();

      _ = hubConnection.DisposeAsync();
    }
  }
}