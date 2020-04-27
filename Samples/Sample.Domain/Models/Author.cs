using System.Collections.Generic;
using Joker.Domain;
using Microsoft.OData.Client;

namespace Sample.Domain.Models
{
  [Key(nameof(Id))]
  public class Author : DomainEntity
  {
    public Author()
    {
      Books = new List<Book>();
    }
    
    public string LastName { get; set; }    
    
    public List<Book> Books { get; set; }

    public Author Clone()
    {
      return MemberwiseClone() as Author;
    }
  }
}