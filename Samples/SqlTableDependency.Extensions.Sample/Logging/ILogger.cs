using System;

namespace SqlTableDependency.Extensions.Sample.Logging
{
  internal interface ILogger
  {
    void Error(Exception error);
    void Trace(string message);
  }
}