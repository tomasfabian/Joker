using Domain.Models;
using Joker.Contracts.Data;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.EFCore
{
  public interface ISampleDbContext : IContext, IDbTransactionFactory
  {
    public DbSet<Book> Books { get; set; }
  }
}