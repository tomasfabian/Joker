using Joker.AspNetCore.MongoDb.Models;

namespace Joker.AspNetCore.MongoDb.Services
{
  public interface ICarService : IMongoService<Car>
  {
  }
}