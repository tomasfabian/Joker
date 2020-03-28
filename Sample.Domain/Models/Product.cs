using Joker.Domain;

namespace Sample.Domain.Models
{
  public class Product : DomainEntity
  {
    public string Name { get; set; }    
    
    public Product Clone()
    {
      return MemberwiseClone() as Product;
    }
  }
}