using System;
using System.Linq;
using System.Linq.Expressions;
using Joker.Contracts.Data;
using Joker.OData.Extensions.Expressions;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.UriParser;

namespace Joker.OData.Controllers
{
  public abstract class ReadOnlyODataController<TEntity> : ODataController
    where TEntity : class
  {
    #region Fields

    private readonly IReadOnlyRepository<TEntity> repository;

    #endregion

    #region Constructors

    protected ReadOnlyODataController(IReadOnlyRepository<TEntity> repository)
    {
      this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    #endregion

    #region Properties

    public FilterQueryValidator FilterQueryValidator { get; set; }

    public SelectExpandQueryValidator SelectExpandQueryValidator { get; set; }

    public int MaxExpansionDepth { get; set; }

    #endregion

    #region Get

    [EnableQuery(MaxExpansionDepth = 3)]
    public OkObjectResult Get(ODataQueryOptions<TEntity> queryOptions)
    {
      AuthenticateQuery(queryOptions);

      var entities = OnGetAll(queryOptions);

      var odataPath = Request.ODataFeature().Path;

      if (odataPath.PathTemplate == @"~/entityset/key")
      {
        var keyPredicate = CreateKeysPredicate(GetKeysFromPath());

        if (keyPredicate != null)
          entities = entities.Where(keyPredicate);
      }

      return Ok(entities);
    }

    protected virtual IQueryable<TEntity> OnGetAll(ODataQueryOptions<TEntity> queryOptions)
    {
      return repository.GetAll();
    }

    protected IQueryable<TEntity> GetAll()
    {
      return repository.GetAll();
    }

    #endregion

    #region CreateKeysPredicate

    protected virtual Expression<Func<TEntity, bool>> CreateKeysPredicate(params object[] keys)
    {
      return ExpressionExtensions.CreatePredicate<TEntity>(keys);
    }

    #endregion

    #region GetKeysFromPath

    protected object[] GetKeysFromPath()
    {
      var odataPath = Request.ODataFeature().Path;

      var value = GetKeysFromPath(odataPath);

      return value;
    }

    protected object[] GetKeysFromPath(Microsoft.AspNet.OData.Routing.ODataPath odataPath)
    {
      var keySegment = odataPath.Segments.OfType<KeySegment>().LastOrDefault();
      if (keySegment == null)
        throw new InvalidOperationException("The link does not contain a key.");

      var value = keySegment.Keys.Select(c => c.Value).ToArray();

      return value;
    }

    #endregion

    private void AuthenticateQuery(ODataQueryOptions<TEntity> queryOptions)
    {
      ValidateQueryOptions(queryOptions);
    }

    #region ValidateQueryOptions

    protected void ValidateQueryOptions(ODataQueryOptions<TEntity> queryOptions)
    {
      OnValidateQueryOptions(queryOptions);

      queryOptions.Validate(new ODataValidationSettings { MaxExpansionDepth = MaxExpansionDepth });
    }

    #endregion

    #region OnValidateQueryOptions

    protected virtual void OnValidateQueryOptions(ODataQueryOptions<TEntity> queryOptions)
    {
      if (SelectExpandQueryValidator != null && queryOptions.SelectExpand != null)
      {
        queryOptions.SelectExpand.Validator = SelectExpandQueryValidator;
      }

      if (FilterQueryValidator != null && queryOptions.Filter != null)
      {
        queryOptions.Filter.Validator = FilterQueryValidator;
      }
    }

    #endregion
  }
}