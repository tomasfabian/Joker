using System.Collections.Generic;

namespace Sample.Domain.Models
{
  [Microsoft.OData.Client.Key(nameof(PublisherId1), nameof(PublisherId2))]
  public class Publisher
  {
    public Publisher()
    {
      Books = new List<Book>();
    }

    [System.ComponentModel.DataAnnotations.Key]
    public int PublisherId1 { get; set; }

    [System.ComponentModel.DataAnnotations.Key]
    public int PublisherId2 { get; set; }

    public string Title { get; set; }    

    public List<Book> Books { get; set; }
    
    public Publisher Clone()
    {
      return MemberwiseClone() as Publisher;
    }
  }
}