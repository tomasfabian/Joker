using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SelfHostedODataService.Joker.EF.Controllers.WebApi
{
  public class HomeController : ControllerBase
  {
    //https://localhost:5001/api/Home/Get
    [HttpGet]
    public IEnumerable<string> Get()
    {
      return new[] { "Joker.OData", "Joker.MVVM", "Joker.Redis" }
        .ToArray();
    }
  }
}