namespace SqlTableDependency.Extensions
{
  public interface ISqlTableDependencyProvider
  {
    void SubscribeToEntityChanges();
  }
}