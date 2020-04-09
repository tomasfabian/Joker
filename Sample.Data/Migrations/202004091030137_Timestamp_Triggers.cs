using Sample.Domain.Models;

namespace Sample.Data.Migrations
{
  public partial class Timestamp_Triggers : CreatedAndModifiedMigration
  {
    protected override string[] DomainEntities =>
      new[] {
          nameof(Product),
      };
  }
}