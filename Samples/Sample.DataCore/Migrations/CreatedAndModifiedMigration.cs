using System;
using System.Collections.Generic;
using System.Linq;
using Joker.Domain;
using Microsoft.EntityFrameworkCore.Migrations;
using Pluralize.NET;

namespace Sample.DataCore.Migrations
{
  public abstract class CreatedAndModifiedMigration : Migration
  {
    private static readonly IPluralize EnglishPluralizationService = new Pluralizer();

    protected bool AddBeforeInsertTrigger { get; set; }

    protected abstract string[] DomainEntities { get; }

    private IEnumerable<string> PluralizedEntityNames => DomainEntities.Select(domainEntity => EnglishPluralizationService.Pluralize(domainEntity));

    protected override void Up(MigrationBuilder migrationBuilder)
    {      
      foreach (var entitySetName in PluralizedEntityNames)
      {
        if(AddBeforeInsertTrigger)
          migrationBuilder.Sql(CreateTrigger(entitySetName, "insert", nameof(DomainEntity.Timestamp)));
        
        migrationBuilder.Sql(CreateTrigger(entitySetName, "update", nameof(DomainEntity.Timestamp)));
      }
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      foreach (var entitySetName in PluralizedEntityNames)
      {
        migrationBuilder.Sql(CreateDropTrigger(entitySetName + "BI"));
        migrationBuilder.Sql(CreateDropTrigger(entitySetName + "BU"));
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