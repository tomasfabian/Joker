using Microsoft.EntityFrameworkCore;
using Sample.DataCore.EFCore;

namespace Sample.DataCore.DesignTime
{
  public class DesignTimeDbContextFactory : DesignTimeDbContextFactory<SampleDbContextCore>
  {
    protected override SampleDbContextCore Create(DbContextOptions<SampleDbContextCore> options)
    {
      return new SampleDbContextCore(options);
    }
  }
}