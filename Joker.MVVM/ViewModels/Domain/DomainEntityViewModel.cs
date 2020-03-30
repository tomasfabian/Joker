using System;
using Joker.Domain;

namespace Joker.MVVM.ViewModels.Domain
{
  public class DomainEntityViewModel<TModel> : ViewModel<TModel>, IDomainEntityViewModel<TModel>
    where TModel: class, IDomainEntity
  {
    public DomainEntityViewModel(TModel model)
      : base(model)
    {
    }

    public int Id => Model.Id;

    public DateTime Timestamp
    {
      get => Model.Timestamp;

      set
      {
        if(value == Model.Timestamp)
          return;

        Model.Timestamp = value;

        NotifyPropertyChanged();
      }
    }

    public void UpdateFrom(TModel updatedModel)
    {
      if(updatedModel == null || updatedModel.Id != Model.Id)
        return;

      OnUpdateFrom(updatedModel);
    }

    protected virtual void OnUpdateFrom(TModel updatedModel)
    {
      Timestamp = updatedModel.Timestamp;
    }
  }
}