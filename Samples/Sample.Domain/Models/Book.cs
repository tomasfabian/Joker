using System.Collections.Generic;
using Joker.Domain;

namespace Sample.Domain.Models
{
  [Microsoft.OData.Client.Key(nameof(Id))]
  public class Book : DomainEntity<string>
  {
    public Book()
    {
      Authors = new List<Author>();
    }

    public string Title { get; set; }    

    public List<Author> Authors { get; set; }

    public Publisher Publisher { get; set; }

    public int? PublisherId1 { get; set; }

    public int? PublisherId2 { get; set; }

    public Book Clone()
    {
      return MemberwiseClone() as Book;
    }
  }
}