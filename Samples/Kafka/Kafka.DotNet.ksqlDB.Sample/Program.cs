using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Sample.Model;
using Kafka.DotNet.ksqlDB.Sample.Observers;

namespace Kafka.DotNet.ksqlDB.Sample
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      Console.WriteLine("Subscription started");

      var ksqlDbUrl = @"http:\\localhost:8088";

      using var disposable = new KQueryStreamSet<Tweet>(new QbservableProvider(ksqlDbUrl))
        .Where(p => p.Message != "Hello world" || p.Id == 1)
        .Select(l => new { l.Message, l.Id })
        .Take(2)     
        .ToObservable() // client side processing starts here lazily after subscription
        .ObserveOn(TaskPoolScheduler.Default)

        .Subscribe(tweetMessage =>
        {
          Console.WriteLine($"{nameof(Tweet)}: {tweetMessage.Id} - {tweetMessage.Message}");
        }, error => { Console.WriteLine($"Exception: {error.Message}"); }, () => Console.WriteLine("Completed"));

      Console.WriteLine("Press any key to stop the subscription");

      Console.ReadKey();

      Console.WriteLine("Subscription completed");
    }

    private static IDisposable KQueryWithObserver(string ksqlDbUrl)
    {
      var subscriptions = new KQueryStreamSet<Tweet>(new QbservableProvider(ksqlDbUrl))
        .Where(p => p.Message != "Hello world" && p.Id != 1)
        .Take(2)
        .Subscribe(new TweetsObserver());

      return subscriptions;
    }
    private static IDisposable ToObservableExample(string ksqlDbUrl)
    {
      var subscriptions = new KQueryStreamSet<Tweet>(new QbservableProvider(ksqlDbUrl))
        .ToObservable()
        .Delay(TimeSpan.FromSeconds(2)) // IObservable extensions
        .Subscribe(new TweetsObserver());

      return subscriptions;
    }
  }
}