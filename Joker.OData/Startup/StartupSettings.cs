namespace Joker.OData.Startup
{
  public class StartupSettings
  {
    public bool UseHttpsRedirection { get; private set; } = true;

    public bool UseUtcTimeZone { get; private set; } = true;

    public bool UseDeveloperExceptionPage { get; set; } = true;

    public bool UseAuthorization { get; set; } = true;

    public bool UseAuthentication { get; set; } = true;

    public bool IISAllowSynchronousIO { get; set; } = true;
    
    public StartupSettings EnableHttpsRedirection()
    {
      UseHttpsRedirection = true;

      return this;
    }
    
    public StartupSettings DisableHttpsRedirection()
    {
      UseHttpsRedirection = false;

      return this;
    }

    public StartupSettings DoNotUseUtcTimeZone()
    {
      UseUtcTimeZone = false;

      return this;
    }
  }
}