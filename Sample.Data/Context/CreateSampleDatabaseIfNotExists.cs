using System.Data.Entity;

namespace Sample.Data.Context
{
  public class CreateSampleDatabaseIfNotExists : CreateDatabaseIfNotExists<SampleDbContext>
  {
    protected override void Seed(SampleDbContext context)
    {
      context.SaveChanges();

      base.Seed(context);
    }
  }
}