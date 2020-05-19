using DataAccessLayer.EFCore;
using Joker.EntityFrameworkCore.DesignTime;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.DesignTime
{
  public class DesignTimeDbContextFactory : DesignTimeDbContextFactory<SampleDbContextCore>
  {
    public DesignTimeDbContextFactory()
    {
    }

    protected override SampleDbContextCore Create(DbContextOptions<SampleDbContextCore> options)
    {
      return new SampleDbContextCore(options);
    }
  }
}