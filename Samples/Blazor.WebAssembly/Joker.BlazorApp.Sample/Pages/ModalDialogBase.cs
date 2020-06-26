using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Joker.BlazorApp.Sample.Navigation;
using Joker.Disposables;
using Joker.Extensions.Disposables;
using Microsoft.AspNetCore.Components;

namespace Joker.BlazorApp.Sample.Pages
{
  public class ModalDialogBase : ComponentBase
  {
    [Inject]
    public IBlazorDialogManager BlazorDialogManager { get; set; }
    
    private DisposableObject disposable;

    protected override Task OnInitializedAsync()
    {
      disposable = DisposableObject.Create(Dispose);

      BlazorDialogManager.ErrorMessages.Subscribe(errorMessage =>
      {
        Message = errorMessage;
        ShowPopup = true;
      }).DisposeWith(disposable.CompositeDisposable);

      return base.OnInitializedAsync();
    }

    private bool showPopup;

    [Parameter]
    public bool ShowPopup
    {
      get => showPopup;
      set
      {
        if(showPopup == value)
          return;

        showPopup = value;

        StateHasChanged();
      }
    }

    [Parameter]
    public string Message { get; set; }

    [Parameter]
    public EventCallback<bool> ShowPopupChanged { get; set; }

    protected Task ClosePopup()
    {
      ShowPopup = false;

      return ShowPopupChanged.InvokeAsync(ShowPopup);
    }    

    public void Dispose()
    {
      using (disposable)
      { }
    }
  }
}