using System;
using System.Data.Entity;

namespace Sample.Data.Context
{
  public class CreateSampleDatabaseIfNotExists : CreateDatabaseIfNotExists<SampleDbContext>
  {
  }
}