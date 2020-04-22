using System.ComponentModel.DataAnnotations.Schema;
using Joker.Domain;
using Microsoft.OData.Client;

namespace Sample.Domain.Models
{
  [Key(nameof(Id))]
  [Table("Products", Schema = "dbo")]
  public class ProductWithTableAttribute : DomainEntity
  {
    [Column("Name")]
    public string ReNameD { get; set; }    
    
    public ProductWithTableAttribute Clone()
    {
      return MemberwiseClone() as ProductWithTableAttribute;
    }
  }
}