using Joker.Platforms.Factories.Schedulers;
using Joker.PubSubUI.Shared.ViewModels.Products;
using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Joker.Extensions;
using Joker.Extensions.Disposables;
using Joker.Notifications;

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

    #endregion
    
    private readonly CompositeDisposable compositeDisposable = new();

    protected override async Task OnInitializedAsync()
    {
      SubscribeToPropertyChanges();
      
      await DomainEntitiesSubscriber.Subscribe();

      await base.OnInitializedAsync();
    }

    private void SubscribeToPropertyChanges()
    {
      Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ViewModel.PropertyChanged += h,
          h => ViewModel.PropertyChanged -= h)
        .Where(c => c.EventArgs.PropertyName.IsOneOfFollowing(nameof(ProductsEntityChangesViewModel.ListViewModel), nameof(ProductsEntityChangesViewModel.IsOffline)))
        .Where(c => ViewModel.ProductsListViewModel != null)
        .Subscribe(c =>
        {
          SubscribeToInnerPropertyChanges();

          StateHasChanged();
        })
        .DisposeWith(compositeDisposable);
    }

    private readonly SerialDisposable productWasUpdatedSubscription = new();

    private void SubscribeToInnerPropertyChanges()
    {
      var refreshesCompositeDisposable = new CompositeDisposable();

      Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
          h => (ViewModel.ProductsListViewModel.Items as INotifyCollectionChanged).CollectionChanged += h,
          h => (ViewModel.ProductsListViewModel.Items as INotifyCollectionChanged).CollectionChanged -= h)
        .Subscribe(_ => StateHasChanged()).DisposeWith(refreshesCompositeDisposable);

      Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ViewModel.PropertyChanged += h,
          h => ViewModel.PropertyChanged -= h)
        .Subscribe(_ => StateHasChanged()).DisposeWith(refreshesCompositeDisposable);

      ViewModel.ProductsListViewModel.SelectionChanged
        .Subscribe(c => { StateHasChanged(); })
        .DisposeWith(refreshesCompositeDisposable);

      ViewModel.ProductsListViewModel.ProductWasUpdated
        .Throttle(TimeSpan.FromMilliseconds(200), PlatformSchedulersFactory.CurrentThread)
        .Subscribe(_ => StateHasChanged())
        .DisposeWith(refreshesCompositeDisposable);

      productWasUpdatedSubscription.Disposable = refreshesCompositeDisposable;
    }

    public void UpdateProduct()
    {
      var selectedProduct = ViewModel.ProductsListViewModel?.SelectedItem;

      if (selectedProduct != null && selectedProduct.Update.CanExecute(null))
        selectedProduct.Update.Execute(null);
    }

    public void Dispose()
    {
      productWasUpdatedSubscription.Dispose();
      compositeDisposable.Dispose();
    }
  }
}