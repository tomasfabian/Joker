using Joker.EntityFrameworkCore.DesignTime;
using Microsoft.EntityFrameworkCore;
using Sample.DataCore.EFCore;

namespace Sample.DataCore.DesignTime
{
  public class DesignTimeDbContextFactory : DesignTimeDbContextFactory<SampleDbContextCore>
  {
    public DesignTimeDbContextFactory()
    {
      ConnectionStringName = "FargoEntities";
    }

    protected override SampleDbContextCore Create(DbContextOptions<SampleDbContextCore> options)
    {
      return new SampleDbContextCore(options);
    }
  }
}