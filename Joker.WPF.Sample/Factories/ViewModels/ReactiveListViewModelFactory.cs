using System.Configuration;
using Joker.MVVM.ViewModels;
using Joker.Reactive;
using Joker.WPF.Sample.Factories.Schedulers;
using Joker.WPF.Sample.ViewModels.Products;
using Joker.WPF.Sample.ViewModels.Reactive;
using Sample.Data.Context;
using Sample.Domain.Models;

namespace Joker.WPF.Sample.Factories.ViewModels
{
  public class ReactiveListViewModelFactory : IReactiveListViewModelFactory<ProductViewModel>
  {
    public ReactiveListViewModelFactory(ReactiveDataWithStatus<Product> reactiveDataWithStatus)
    {
      ReactiveDataWithStatus = reactiveDataWithStatus;
    }

    private ReactiveDataWithStatus<Product> ReactiveDataWithStatus { get; set; }
    
    public IReactiveListViewModel<ProductViewModel> Create()
    {      
      string connectionString = ConfigurationManager.ConnectionStrings["FargoEntities"].ConnectionString;
      var sampleDbContext = new SampleDbContext(connectionString);
      
      var schedulersFactory = new WpfSchedulersFactory();

      var reactiveProductsViewModel = new ReactiveProductsViewModel(sampleDbContext, ReactiveDataWithStatus, schedulersFactory);

      return reactiveProductsViewModel;
    }
  }
}