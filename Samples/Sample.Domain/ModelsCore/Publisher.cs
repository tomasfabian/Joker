using System.Collections.Generic;
using Joker.Domain;

namespace Sample.Domain.ModelsCore
{
  public class Publisher : DomainEntity
  {    
    public string Title { get; set; }    

    public List<Book> Books { get; set; }
  }
}