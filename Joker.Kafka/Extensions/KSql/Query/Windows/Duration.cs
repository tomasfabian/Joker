namespace Kafka.DotNet.ksqlDB.Extensions.KSql.Query.Windows
{
  public class Duration
  {
    public TimeUnits TimeUnit { get; private set; }
    public int Value { get; private set; }

    public static Duration OfSeconds(int seconds)
    {
      return new()
      {
        TimeUnit = TimeUnits.SECONDS,
        Value = seconds
      };
    }

    public static Duration OfMinutes(int minutes)
    {
      return new()
      {
        TimeUnit = TimeUnits.MINUTES,
        Value = minutes
      };
    }

    public static Duration OfHours(int hours)
    {
      return new()
      {
        TimeUnit = TimeUnits.HOURS,
        Value = hours
      };
    }
  }
}