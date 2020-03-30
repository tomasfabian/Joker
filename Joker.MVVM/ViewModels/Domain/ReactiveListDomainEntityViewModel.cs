using System.Collections.Generic;
using Joker.Comparators;
using Joker.Contracts;
using Joker.Domain;
using Joker.Factories.Schedulers;

namespace Joker.MVVM.ViewModels.Domain
{
  public abstract class ReactiveListDomainEntityViewModel<TDomainEntity, TDomainEntityViewModel> : ReactiveListViewModel<TDomainEntity, TDomainEntityViewModel>
    where TDomainEntity : class, IDomainEntity
    where TDomainEntityViewModel : class, IDomainEntityViewModel<TDomainEntity>
  {
    protected ReactiveListDomainEntityViewModel(IReactiveData<TDomainEntity> reactive, ISchedulersFactory schedulersFactory) 
      : base(reactive, schedulersFactory)
    {
      Comparer = new DomainEntityComparer();
    }

    protected override IEqualityComparer<TDomainEntity> Comparer { get; }
  }
}