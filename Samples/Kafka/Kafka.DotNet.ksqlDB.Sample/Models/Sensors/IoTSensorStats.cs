namespace Kafka.DotNet.ksqlDB.Sample.Models.Sensors
{
  public record IoTSensorStats
  {
    public string SensorId { get; set; }
    public double AvgValue { get; set; }
  }
}