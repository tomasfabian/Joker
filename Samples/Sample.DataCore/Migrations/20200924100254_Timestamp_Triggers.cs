using Sample.Domain.Models;

namespace Sample.DataCore.Migrations
{
  public partial class Timestamp_Triggers : CreatedAndModifiedMigration
  {
    protected override string[] DomainEntities =>
      new[] {
        nameof(Product),
      };
  }
}