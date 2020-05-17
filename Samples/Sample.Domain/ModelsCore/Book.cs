using Joker.Domain;

namespace Sample.Domain.ModelsCore
{
  public class Book : DomainEntity
  {
    public string Title { get; set; }    
    
    public int PublisherId { get; set; }

    public Publisher Publisher { get; set; }
  }
}