using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Joker.OData.Middleware.Logging
{
  public class ErrorLoggerMiddleware
  {
    private readonly RequestDelegate nextRequest;
    private readonly ILogger logger;

    public ErrorLoggerMiddleware(RequestDelegate nextRequest, ILoggerFactory loggerFactory)
    {
      this.nextRequest = nextRequest;
      logger = loggerFactory.CreateLogger("ErrorLoggerMiddleware");
    }

    public async Task Invoke(HttpContext context)
    {
      try
      {
        await nextRequest(context);
      }
      catch (Exception exception)
      {
        logger.LogError(exception, nameof(ErrorLoggerMiddleware), context);

        throw;
      }
    }
  }
}