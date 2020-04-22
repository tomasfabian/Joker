using System.Diagnostics;

namespace SqlTableDependency.Extensions.IntegrationTests
{
  internal static class ProcessProvider
  {
    internal static void Docker(string command)
    {
      var processInfo = new ProcessStartInfo("docker", command)
      {
        CreateNoWindow = true, 
        UseShellExecute = false, 
        RedirectStandardOutput = true, 
        RedirectStandardError = true
      };

      using var process = new Process
      {
        StartInfo = processInfo
      };

      process.Start();

      process.WaitForExit();
      
      if (!process.HasExited)
      {
        process.Kill();
      }
      process.Close();
    }
  }
}