using System.Data.Entity;
using Domain.Models;
using Joker.Contracts.Data;

namespace DataAccessLayer.Context
{
  public interface ISampleDbContext : IContext, IDbTransactionFactory
  {
    IDbSet<Book> Books { get; set; }
  }
}