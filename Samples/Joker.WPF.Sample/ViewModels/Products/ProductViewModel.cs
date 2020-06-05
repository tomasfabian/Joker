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
    private readonly IPlatformSchedulersFactory schedulersFactory;

    [Inject]
    public ProductViewModel(Product product, IPlatformSchedulersFactory schedulersFactory)
      : base(product)
    {
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));
    }

    public ProductViewModel(Product product)
      : this(product, new PlatformSchedulersFactory())
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

    private string renamed;

    public string Renamed
    {
      get => renamed;
      set
      {
        SetProperty(ref renamed, value);
        update?.RaiseCanExecuteChanged();
      }
    }

    private bool isActive;

    public bool IsActive
    {
      get => isActive;
      set
      {
        if (isActive == value)
          return;

        isActive = value;

        Renamed = Name;

        update?.RaiseCanExecuteChanged();

        NotifyPropertyChanged();
      }
    }

    #region Commands

    #region Update

    private DelegateCommand update;

    public ICommand Update => update ?? (update = new DelegateCommand(OnUpdate, OnCanUpdate));

    private bool OnCanUpdate()
    {
      return Name != Renamed;
    }

    private void OnUpdate()
    {
      var dataServiceContext = new ODataServiceContextFactory().CreateODataContext();
      
      var product = Model.Clone();
      
      dataServiceContext.AttachTo("Products", product);

      product.Name = Renamed;

      dataServiceContext.UpdateObject(product);
      
      dataServiceContext.SaveChangesAsync()
        .ToObservable()
        .ObserveOn(schedulersFactory.Dispatcher)
        .Subscribe(_ => {  }, error => MessageBox.Show($"Failed to update {Name} to {Renamed}"));
    }

    #endregion

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