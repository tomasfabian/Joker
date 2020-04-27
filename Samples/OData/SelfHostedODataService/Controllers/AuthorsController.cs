using Joker.Contracts.Data;
using Joker.OData.Controllers;
using Sample.Domain.Models;

namespace SelfHostedODataService.Controllers
{
  public class AuthorsController : ODataControllerBase<Author>
  {
    public AuthorsController(IRepository<Author> repository)
      : base(repository)
    {
    }
  }
}