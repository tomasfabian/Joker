using Joker.Domain;

namespace Domain.Models
{
  [Microsoft.OData.Client.Key(nameof(Id))]
  public class Book : DomainEntity
  {
    public string Title { get; set; }

    public Book Clone()
    {
      return MemberwiseClone() as Book;
    }
  }
}