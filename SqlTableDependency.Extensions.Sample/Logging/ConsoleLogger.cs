using System;

namespace SqlTableDependency.Extensions.Sample.Logging
{
  internal class ConsoleLogger : ILogger
  {
    public void Error(Exception error)
    {
      Console.ForegroundColor = ConsoleColor.Red;

      Console.WriteLine($"Error: {error.Message}");

      Console.ResetColor();
    }

    public void Trace(string message)
    {
      Console.ForegroundColor = ConsoleColor.Yellow;

      Console.WriteLine($"Info: {message}");

      Console.ResetColor();
    }
  }
}