using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Joker.MVVM.ViewModels.Domain;
using Joker.WPF.Sample.Factories.Schedulers;
using Ninject;
using OData.Client;
using Prism.Commands;
using Sample.Domain.Models;

namespace Joker.WPF.Sample.ViewModels.Products
{
  public class ProductViewModel : DomainEntityViewModel<Product>
  {
    private readonly IWpfSchedulersFactory schedulersFactory;

    [Inject]
    public ProductViewModel(Product product, IWpfSchedulersFactory schedulersFactory)
      : base(product)
    {
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));
    }

    public ProductViewModel(Product product)
      : this(product, new WpfSchedulersFactory())
    {
    }

    public string Name
    {
      get => Model.Name;
      set
      {
        if (Model.Name == value)
          return;

        Model.Name = value;

        NotifyPropertyChanged();
      }
    }
    
    #region Commands

    #region Delete

    private DelegateCommand delete;

    public ICommand Delete => delete ?? (delete = new DelegateCommand(OnDelete, OnCanDelete));

    private bool OnCanDelete()
    {
      return true;
    }

    private void OnDelete()
    {
      var dataServiceContext = new ODataServiceContextFactory().CreateODataContext();

      dataServiceContext.AttachTo("Products", Model);
      dataServiceContext.DeleteObject(Model);
      
      dataServiceContext.SaveChangesAsync()
        .ToObservable()
        .ObserveOn(schedulersFactory.Dispatcher)
        .Subscribe(_ => {  }, error => MessageBox.Show($"Failed to delete {Name}"));
    }

    #endregion

    #endregion

    protected override void OnUpdateFrom(Product updatedModel)
    {
      base.OnUpdateFrom(updatedModel);

      Name = updatedModel.Name;
    }
  }
}