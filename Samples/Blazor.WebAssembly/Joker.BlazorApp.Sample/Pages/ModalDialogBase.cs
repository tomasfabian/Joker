using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Joker.BlazorApp.Sample.Pages
{
  public class ModalDialogBase : ComponentBase
  {
    public ModalDialogBase()
    {
      ShowPopup = true;
    }

    public bool ShowPopup { get; set; }
    public string Message { get; set; }

    public void ClosePopup()
    {
      ShowPopup = false;
    }
  }
}