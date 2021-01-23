using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Linq;
using Kafka.DotNet.ksqlDB.Extensions.KSql.Query;
using Kafka.DotNet.ksqlDB.Sample.Model;
using Kafka.DotNet.ksqlDB.Sample.Observers;
using Kafka.DotNet.ksqlDB.Sample.QueryStreams;

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
        //.Where(p => p.RowTime >= 1510923225000) //AND RowTime >= 1510923225000
        .Select(l => new { l.Id, l.Message })
        .Take(2)     
        .ToObservable() // client side processing starts here lazily after subscription
        .ObserveOn(TaskPoolScheduler.Default)
        .Subscribe(tweetMessage =>
        {
          Console.WriteLine($"{nameof(Tweet)}: {tweetMessage.Id} - {tweetMessage.Message}");
          Console.WriteLine();
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

    private static void ToQueryStringExample(string ksqlDbUrl)
    {
      var ksql = new TweetsQueryStream(ksqlDbUrl).ToQueryString();

      //prints SELECT * FROM Tweets EMIT CHANGES;
      Console.WriteLine(ksql);

      ksql = new PeopleQueryStream(ksqlDbUrl).ToQueryString();

      //prints SELECT * FROM People EMIT CHANGES;
      Console.WriteLine(ksql);
    }
  }
}