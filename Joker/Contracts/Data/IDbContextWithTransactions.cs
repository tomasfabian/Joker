namespace Joker.Contracts.Data
{
  public interface IDbContextWithTransactions : IContext, IDbTransactionFactory
  {
  }
}