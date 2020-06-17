using Joker.Platforms.Factories.Schedulers;
using Joker.PubSubUI.Shared.ViewModels.Products;
using Joker.Redis.ConnectionMultiplexers;
using Microsoft.AspNetCore.Components;
using Sample.Domain.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Joker.Redis.Notifications;

namespace Joker.BlazorApp.Sample.Pages
{
  public class ProductsComponentBase : ComponentBase
  {
    //[Inject]
    public ProductsEntityChangesViewModel ViewModel { get; set; }

    [Inject]
    public IPlatformSchedulersFactory PlatformSchedulersFactory { get; set; }
    
    [Inject]
    public IRedisSubscriber RedisSubscriber { get; set; }

    [Inject]
    public IDomainEntitiesSubscriber DomainEntitiesSubscriber { get; set; }

    public ObservableCollection<Product> Products { get; set; }
    
    public Product SelectedProduct { get; set; }

    protected override async Task OnInitializedAsync()
    {
      Products = new ObservableCollection<Product>()
      {
        new Product { Id = -1, Name = "Product 1"},
        new Product { Id = -2, Name = "Product 2"},
      };

      SelectedProduct = Products.FirstOrDefault();

      await base.OnInitializedAsync();
    }

    public Product NewProduct { get; set; } = new Product();

    public void Add()
    {
      NewProduct.Id = i++;

      Products.Add(NewProduct);

      NewProduct = new Product();
    }

    private SerialDisposable sd = new SerialDisposable();
    
    public string StatusClass { get; set; }
    public string Message { get; set; }
    private int i;

    public void UpdateProduct()
    {
      //Products[0].Name = "et " + i;

      if (true)
      {
        StatusClass = "alert-success";
        
        Message = string.Empty;
      }
      else
      {
        StatusClass = "alert-danger";

        Message = "Error occured";
      }
    }

    public void Delete(ProductViewModel product)
    {
      //ViewModel.ProductsListViewModel.Items.Remove(product);
    }

    public void Delete(Product product)
    {
      Products.Remove(product);
    }
  }
}