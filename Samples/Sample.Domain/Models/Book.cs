using System.Collections.Generic;
using Joker.Domain;
using Microsoft.OData.Client;

namespace Sample.Domain.Models
{
  [Key(nameof(Id))]
  public class Book : DomainEntity<string>
  {
    public Book()
    {
      Authors = new List<Author>();
    }

    public string Title { get; set; }    

    public List<Author> Authors { get; set; }
    
    public Book Clone()
    {
      return MemberwiseClone() as Book;
    }
  }
}