﻿namespace Kafka.DotNet.ksqlDB.IntegrationTests.Models.Sensors
{
  public record IoTSensor
  {
    public string SensorId { get; set; }
    public int Value { get; set; }
  }
}