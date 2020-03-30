using Joker.MVVM.ViewModels.Domain;
using Sample.Domain.Models;

namespace Joker.WPF.Sample.ViewModels.Products
{
  public class ProductViewModel : DomainEntityViewModel<Product>
  {
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

    protected override void OnUpdateFrom(Product updatedModel)
    {
      base.OnUpdateFrom(updatedModel);

      Name = updatedModel.Name;
    }
  }
}