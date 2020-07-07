using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows.Input;
using Joker.MVVM.ViewModels.Domain;
using Joker.Platforms.Factories.Schedulers;
using Joker.PubSubUI.Shared.Navigation;
using OData.Client;
using Prism.Commands;
using Sample.Domain.Models;

namespace Joker.PubSubUI.Shared.ViewModels.Products
{
  public class ProductViewModel : DomainEntityViewModel<Product>
  {
    private readonly IODataServiceContextFactory dataServiceContextFactory;
    private readonly IPlatformSchedulersFactory schedulersFactory;
    private readonly IDialogManager dialogManager;

    public ProductViewModel(
      Product product,
      IODataServiceContextFactory dataServiceContextFactory,
      IPlatformSchedulersFactory schedulersFactory,
      IDialogManager dialogManager)
      : base(product)
    {
      this.dataServiceContextFactory = dataServiceContextFactory ?? throw new ArgumentNullException(nameof(dataServiceContextFactory));
      this.schedulersFactory = schedulersFactory ?? throw new ArgumentNullException(nameof(schedulersFactory));
      this.dialogManager = dialogManager ?? throw new ArgumentNullException(nameof(dialogManager));
    }

    public ProductViewModel(Product product)
      : base(product)
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
      var dataServiceContext = dataServiceContextFactory.CreateODataContext();
      
      var product = Model.Clone();
      
      dataServiceContext.AttachTo("Products", product);

      product.Name = Renamed;

      dataServiceContext.UpdateObject(product);
      
      dataServiceContext.SaveChangesAsync()
        .ToObservable()
        .ObserveOn(schedulersFactory.Dispatcher)
        .Subscribe(_ => {  }, error => dialogManager.ShowMessage($"Failed to update {Name} to {Renamed}"));
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
      var dataServiceContext = dataServiceContextFactory.CreateODataContext();

      dataServiceContext.AttachTo("Products", Model);
      dataServiceContext.DeleteObject(Model);
      
      dataServiceContext.SaveChangesAsync()
        .ToObservable()
        .ObserveOn(schedulersFactory.Dispatcher)
        .Subscribe(_ => {  }, error => dialogManager.ShowMessage($"Failed to delete {Name}"));
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