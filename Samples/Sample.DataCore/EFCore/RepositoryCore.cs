using System;
using System.Threading.Tasks;
using Joker.Contracts.Data;

namespace Sample.DataCore.EFCore
{
  public abstract class RepositoryCore<TEntity> : ReadOnlyRepositoryCore<TEntity>, IRepository<TEntity> 
    where TEntity : class
  {
    private readonly IContext context;

    protected RepositoryCore(IContext context)
      : base(context)
    {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Add(TEntity entity)
    {
      DbSet.Add(entity);
    }

    public void Update(TEntity entity)
    {
      DbSet.Update(entity);//AddOrUpdate
    }

    public void Remove(params object[] keys)
    {
      var entity = DbSet.Find(keys);
      
      DbSet.Remove(entity);
    }

    public void Remove(TEntity entity)
    {
      DbSet.Remove(entity);
    }

    public Task<int> SaveChangesAsync()
    {
      return context.SaveChangesAsync();
    }
  }
}