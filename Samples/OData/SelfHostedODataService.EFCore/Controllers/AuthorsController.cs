﻿using System;
using Joker.Contracts.Data;
using Joker.OData.Controllers;
using Sample.DataCore.EFCore;
using Sample.Domain.Models;

namespace SelfHostedODataService.EFCore.Controllers
{
  public class AuthorsController : ODataControllerBase<Author>
  {
    private readonly ISampleDbContext dbContext;

    public AuthorsController(IRepository<Author> repository, ISampleDbContext dbContext)
      : base(repository)
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    protected override dynamic TryGetDbSet(Type entityType)
    {
      if (entityType == typeof(Book))
        return dbContext.Books;

      return null;
    }
  }
}