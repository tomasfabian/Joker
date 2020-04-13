using Joker.Domain;
using Microsoft.OData.Client;

namespace Sample.Domain.Models
{
  [Key(nameof(Id))]
  public class Product : DomainEntity
  {
    public string Name { get; set; }    
    
    public Product Clone()
    {
      return MemberwiseClone() as Product;
    }
  }
}