using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Joker.AspNetCore.MongoDb.Models;
using Joker.AspNetCore.MongoDb.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Joker.AspNetCore.MongoDb.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CarsController : Controller
  {
    private readonly ICarMongoRepository carService;

    public CarsController(ICarMongoRepository carService)
    {
      this.carService = carService ?? throw new ArgumentNullException(nameof(carService));

      carService.Initialize();
    }

    [HttpGet]
    public async Task<ActionResult<List<Car>>> Get() =>
      await carService.GetAsync();

    [HttpGet("{id:length(24)}", Name = "GetCar")]
    public async Task<ActionResult<Car>> Get(string id)
    {
      var car = await carService.GetAsync(id);

      if (car == null)
      {
        return NotFound();
      }

      return car;
    }

    [HttpPost]
    public async Task<ActionResult<Car>> Create(Car car)
    {
      await carService.CreateAsync(car);

      return CreatedAtRoute("GetCar", new { id = car.Id }, car);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Car carIn)
    {
      var car = await carService.GetAsync(id);

      if (car == null)
      {
        return NotFound();
      }

      await carService.UpdateAsync(id, carIn);

      return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
      var car = await carService.GetAsync(id);

      if (car == null)
      {
        return NotFound();
      }

      await carService.RemoveAsync(car.Id);

      return NoContent();
    }

    public async Task<ActionResult> Index()
    {
      var buildInfo = await carService.GetBuildInfo();

      return Content(buildInfo.ToJson(), "application/json");
    }
  }
}