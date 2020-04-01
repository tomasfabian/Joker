using System;

namespace Joker.MVVM.Tests.Helpers
{
  public class TestModelProvider
  {
    public readonly TestModel Model1 = new TestModel
    {
      Id = 1,
      Timestamp = new DateTime(2019, 3, 27, 16, 0, 0),
      Name = "Model 1"
    };

    private readonly TestModel originalModel = new TestModel
    {
      Timestamp = new DateTime(2020, 3, 27, 16, 0, 0),
      Name = "Original"
    };

    public TestModel Model2 => Create(2);
    public TestModel Model3 => Create(3);
    public TestModel Model4 => Create(4);

    private TestModel Create(int id)
    {
      var model = originalModel.Clone();

      model.Id = id;
      model.Name = $"{model.Name} {id}";

      return model;
    }
  }
}