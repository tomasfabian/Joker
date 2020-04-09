using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Data.Entity.Migrations;
using System.Linq;
using Joker.Domain;

namespace Sample.Data.Migrations
{
  public abstract class CreatedAndModifiedMigration : DbMigration
  {
    private readonly EnglishPluralizationService englishPluralizationService = new EnglishPluralizationService();

    protected bool AddBeforeInsertTrigger { get; set; }

    protected abstract string[] DomainEntities { get; }

    private IEnumerable<string> PluralizedEntityNames => DomainEntities.Select(domainEntity => englishPluralizationService.Pluralize(domainEntity));

    public override void Up()
    {
      foreach (var entitySetName in PluralizedEntityNames)
      {
        if(AddBeforeInsertTrigger)
          Sql(CreateTrigger(entitySetName, "insert", nameof(DomainEntity.Timestamp)));
        
        Sql(CreateTrigger(entitySetName, "update", nameof(DomainEntity.Timestamp)));
      }
    }

    public override void Down()
    {
      foreach (var entitySetName in PluralizedEntityNames)
      {
        Sql(CreateDropTrigger(entitySetName + "BI"));
        Sql(CreateDropTrigger(entitySetName + "BU"));
      }
    }

    public string CreateDropTrigger(string triggerName)
    {
      return $"DROP TRIGGER [{triggerName}]";
    }

    public string CreateTrigger(string tableName, string operation, string columnName)
    {
      string triggerName = tableName + (operation == "insert" ? "BI" : "BU");

      return $@"CREATE trigger[dbo].[{triggerName}] on[dbo].[{tableName}] for {operation} as
    begin
      declare
    @numrows int

      select  @numrows = @@rowcount
    if @numrows = 0
    return

    UPDATE {tableName} SET {columnName} = GETDATE()
    FROM Inserted i
      WHERE i.Id = {tableName}.Id   

    return

    error:
      rollback transaction
    end";
    }
  }
}