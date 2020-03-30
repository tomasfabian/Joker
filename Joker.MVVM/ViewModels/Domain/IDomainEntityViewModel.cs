using Joker.Contracts;

namespace Joker.MVVM.ViewModels.Domain
{
  public interface IDomainEntityViewModel<out TModel> : IViewModel<TModel> 
    where TModel : IVersion
  {

  }
}