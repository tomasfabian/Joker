using System.Configuration;
using Joker.MVVM.ViewModels;
using Joker.Reactive;
using Joker.WPF.Sample.Factories.Schedulers;
using Joker.WPF.Sample.ViewModels.Products;
using Joker.WPF.Sample.ViewModels.Reactive;
using Ninject;
using Sample.Data.Context;
using Sample.Domain.Models;

namespace Joker.WPF.Sample.Factories.ViewModels
{
  public class ReactiveListViewModelFactory : ViewModelsFactory, IReactiveListViewModelFactory<ProductViewModel>
  {
    public ReactiveListViewModelFactory(IKernel kernel, ReactiveDataWithStatus<Product> reactiveDataWithStatus) 
      : base(kernel)
    {
      ReactiveDataWithStatus = reactiveDataWithStatus;
    }

    private ReactiveDataWithStatus<Product> ReactiveDataWithStatus { get; }
    
    public IReactiveListViewModel<ProductViewModel> Create()
    {
      return Kernel.Get<ReactiveProductsViewModel>();
    }    

    public IReactiveListViewModel<ProductViewModel> CreateWithPoorMansDependencyInjection()
    {      
      string connectionString = ConfigurationManager.ConnectionStrings["FargoEntities"].ConnectionString;

      var sampleDbContext = new SampleDbContext(connectionString);
      
      var schedulersFactory = new WpfSchedulersFactory();

      var reactiveProductsViewModel = new ReactiveProductsViewModel(sampleDbContext, ReactiveDataWithStatus, this, schedulersFactory);

      return reactiveProductsViewModel;
    }
  }
}