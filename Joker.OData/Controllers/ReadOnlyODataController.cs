using System;
using System.Linq;
using System.Linq.Expressions;
using Joker.Contracts.Data;
using Joker.OData.Extensions.Expressions;
using Joker.OData.Extensions.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Validator;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

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

    #endregion

    #region Methods

    #region Get

    //https://localhost:5003/Authors(2)?$expand=Books
    [EnableQuery]
    public SingleResult<TEntity> Get(object key, ODataQueryOptions<TEntity> queryOptions)
    {
      var keyPredicate = CreateKeysPredicate(Request.GetKeysFromODataPath());

      IQueryable<TEntity> result = OnGetAll(queryOptions).Where(keyPredicate);

      return SingleResult.Create(result);
    }

    [EnableQuery]
    public IActionResult Get(ODataQueryOptions<TEntity> queryOptions)
    {
      AuthenticateQuery(queryOptions);

      var entities = OnGetAll(queryOptions);
      
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

    #region AuthenticateQuery

    private void AuthenticateQuery(ODataQueryOptions<TEntity> queryOptions)
    {
      ValidateQueryOptions(queryOptions);
    }

    #endregion

    #region ValidateQueryOptions

    protected void ValidateQueryOptions(ODataQueryOptions<TEntity> queryOptions)
    {
      OnValidateQueryOptions(queryOptions);
    }

    #endregion

    #region OnValidateQueryOptions

    protected virtual void OnValidateQueryOptions(ODataQueryOptions<TEntity> queryOptions)
    {
      if (SelectExpandQueryValidator != null && queryOptions.SelectExpand != null)
        queryOptions.SelectExpand.Validator = SelectExpandQueryValidator;

      if (FilterQueryValidator != null && queryOptions.Filter != null)
        queryOptions.Filter.Validator = FilterQueryValidator;

      var oDataValidationSettings = OnCreateODataValidationSettings();

      queryOptions.Validate(oDataValidationSettings);
    }

    #endregion

    #region OnCreateODataValidationSettings

    protected virtual ODataValidationSettings OnCreateODataValidationSettings()
    {
      var oDataValidationSettings = new ODataValidationSettings();

      return oDataValidationSettings;
    }

    #endregion

    #endregion
  }
}