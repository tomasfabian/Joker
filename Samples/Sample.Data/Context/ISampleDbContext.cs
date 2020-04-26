using System.Data.Entity;
using Joker.Contracts.Data;
using Sample.Domain.Models;

namespace Sample.Data.Context
{
  public interface ISampleDbContext : IContext
  {
    IDbSet<Product> Products { get; set; }
  }
}