using System.ComponentModel.DataAnnotations.Schema;

namespace SqlTableDependency.Extensions.Tests.Models
{
  [Table("TableName", Schema = "TestSchema")]
  public class TestModelWithTableAttribute
  {
    public int Id { get; set; }

    [Column("TestColumnName")]
    public string Name { get; set; }
  }
}