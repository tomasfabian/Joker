using Joker.AspNetCore.MongoDb.Models;
using Joker.AspNetCore.MongoDb.Settings.Database;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace Joker.AspNetCore.MongoDb.Services
{
  public class CarService : MongoService<Car>, ICarService
  {
    public CarService(IDatabaseSettings settings)
      : base(settings)
    {
    }
    
    protected override IEventSubscriber EventSubscriber => new EventSubscriber();

    protected override UpdateDefinition<Car> OnUpdate(Car carIn, UpdateDefinitionBuilder<Car> update, UpdateOptions updateOptions)
    {
      return update.Set(c => c.CarName, carIn.CarName);
    }
  }
}