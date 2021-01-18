using System;
using Kafka.DotNet.ksqlDB.Sample.Model;

namespace Kafka.DotNet.ksqlDB.Sample.Observers
{
  public class TweetsObserver : IObserver<Tweet>
  {
    public void OnCompleted()
    {
      Console.WriteLine($"{nameof(Tweet)}: completed successfully");
    }

    public void OnError(Exception error)
    {
      Console.WriteLine($"{nameof(Tweet)}: {error.Message}");
    }
    
    public void OnNext(Tweet tweetMessage)
    {
      if (tweetMessage == null)
        return;
        
      if(string.IsNullOrEmpty(tweetMessage.Message))
        return;

      Console.WriteLine($"{nameof(Tweet)}: {tweetMessage.Message}");
    }
  }
}