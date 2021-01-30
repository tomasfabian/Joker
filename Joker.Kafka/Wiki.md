This package generates ksql queries from your C# linq queries. You can filter, project and limit your push notifications server side with [ksqlDB push queries](https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-rest-api/streaming-endpoint/)

```
Install-Package Kafka.DotNet.ksqlDB -Version 0.1.0
```
```C#
using System;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.Sample.Model;

var ksqlDbUrl = @"http:\\localhost:8088";

await using var context = new KSqlDBContext(ksqlDbUrl);

using var disposable = context.CreateQueryStream<Tweet>()
  .Where(p => p.Message != "Spoiler" || p.Id == 1)
  .Select(l => new { l.Message, l.Id })
  .Take(2)
  .Subscribe(tweetMessage =>
  {
    Console.WriteLine($"{nameof(Tweet)}: {tweetMessage.Id} - {tweetMessage.Message}");
  }, error => { Console.WriteLine($"Exception: {error.Message}"); }, () => Console.WriteLine("Completed"));

Console.WriteLine("Press any key to stop the subscription");

Console.ReadKey();
```

In the above code snippet everything runs in the server side except of the ``` IQbservable<TEntity>.Subscribe``` method. It subscribes to your ksqlDB stream created in the following manner:
```SQL
CREATE STREAM tweets(id INT, message VARCHAR)
  WITH (kafka_topic='tweetsTopic', value_format='json', partitions=1);
```

LINQ code written in C# from the sample is equivalent to this ksql query:
```SQL
SELECT Message, Id FROM Tweets
WHERE Message != 'Hello world' OR Id = 1 
EMIT CHANGES 
LIMIT 2;
```

Run the following insert statement to stream messages with your ksqldb-cli
```
docker exec -it $(docker ps -q -f name=ksqldb-cli) ksql http://localhost:8088
```
```SQL
INSERT INTO tweets (id, message) VALUES (1, 'Hello world');
```
Sample project can be found under [Examples/Kafka](https://github.com/tomasfabian/Joker/tree/master/Samples/Kafka/Kafka.DotNet.ksqlDB.Sample) solution folder in Joker.sln or Joker.DotNet5.sln 



**External dependencies:**
- kafka broker and ksqlDB

CD to [Examples/Kafka](https://github.com/tomasfabian/Joker/tree/master/Samples/Kafka/Kafka.DotNet.ksqlDB.Sample)

run in command line:

```docker compose up -d```

# Setting query parameters (v.0.1.0)
Default settings:
'auto.offset.reset' is set to 'earliest' by default. 
New parameters could be added or existing ones changed in the following manner:
```C#
var contextOptions = new KSqlDBContextOptions(@"http:\\localhost:8088");

contextOptions.QueryStreamParameters["auto.offset.reset"] = "latest";
```

### Record (row) class (v.0.1.0)
Record class is a base class for rows returned in push queries. It has a 'RowTime' property.

```C#
public class Tweet : Kafka.DotNet.ksqlDB.KSql.Query.Record
{
  public string Message { get; set; }
}

context.CreateQueryStream<Tweet>()
  .Select(c => new { c.RowTime, c.Message });
```

### Overriding stream name (v.0.1.0)
Stream names are generated based on the generic record types. They are pluralized with Pluralize.NET package
```C#
context.CreateQueryStream<Person>();
```
```SQL
FROM People
```
This can be disabled:
```C#
var contextOptions = new KSqlDBContextOptions(@"http:\\localhost:8088")
{
  ShouldPluralizeStreamName = false
};

new KSqlDBContext(contextOptions).CreateQueryStream<Person>();
```
```SQL
FROM Person
```

Setting an arbitrary stream name:
```C#
context.CreateQueryStream<Tweet>("custom_topic_name");
```
```SQL
FROM custom_topic_name
```

# ```IQbservable<T>``` extension methods

### Select (v.0.1.0)
Projects each element of a stream into a new form.
```C#
context.CreateQueryStream<Tweet>()
  .Select(l => new { l.RowTime, l.Message });
```
Omitting select is equivalent to SELECT *
### Supported data types mapping
|   ksql  |   c#   |
|:-------:|:------:|
| STRING  | string |
| INTEGER | int    |
| BIGINT  | long   |
| DOUBLE  | double |
| BOOLEAN | bool   |

### Where (v.0.1.0)
Filters a stream of values based on a predicate.
```C#
context.CreateQueryStream<Tweet>()
  .Where(p => p.Message != "Hello world" || p.Id == 1)
  .Where(p => p.RowTime >= 1510923225000);
```
Multiple Where statements are joined with AND operator. 
```KSQL
SELECT * FROM Tweets
WHERE Message != 'Hello world' OR Id = 1 AND RowTime >= 1510923225000
EMIT CHANGES;
```

Supported operators are:
|   ksql   |           meaning           |  c#  |
|:--------:|:---------------------------:|:----:|
| =        | is equal to                 | ==   |
| != or <> | is not equal to             | !=   |
| <        | is less than                | <    |
| <=       | is less than or equal to    | <=   |
| >        | is greater than             | >    |
| >=       | is greater than or equal to | >=   |
| AND      | logical AND                 | &&   |
| OR       | logical OR                  | \|\| |

### Take (Limit) (v.0.1.0)
Returns a specified number of contiguous elements from the start of a stream. Depends on the 'auto.topic.offset' parameter.
```C#
context.CreateQueryStream<Tweet>()
  .Take(2);
```
```SQL
SELECT * from tweets EMIT CHANGES LIMIT 2;
```

### ToObservable moving to [Rx.NET](https://github.com/dotnet/reactive)
The following code snippet shows how to observe messages on the desired [IScheduler](http://introtorx.com/Content/v1.0.10621.0/15_SchedulingAndThreading.html): 

```C#
using var disposable = context.CreateQueryStream<Tweet>()        
  .Take(2)     
  .ToObservable() //client side processing starts here lazily after subscription
  .ObserveOn(System.Reactive.Concurrency.DispatcherScheduler.Current)
  .Subscribe(new TweetsObserver());
```
Be cautious regarding to server side and client side processings:
```C#
Extensions.KSql.Linq.IQbservable<Tweet> queryStream = context.CreateQueryStream<Tweet>().Take(2);

System.IObservable<Tweet> observable = queryStream.ToObservable();

//All reactive extension methods are available from this point.
//The not obvious difference is that the processing is done client side, not server side (ksqldb) as in the case of IQbservable:
observable.Throttle(TimeSpan.FromSeconds(3)).Subscribe();
```

### ToQueryString (v.0.1.0)
ToQueryString is helpful for debugging purposes. Returns the generated ksql query without executing it.
```C#
var ksql = context.CreateQueryStream<Tweet>().ToQueryString();

//prints SELECT * FROM tweets EMIT CHANGES;
Console.WriteLine(ksql);
```

### GroupBy (v.0.1.0)
#### Count (v.0.1.0)
Count the number of rows. When * is specified, the count returned will be the total number of rows.
```C#
var ksqlDbUrl = @"http:\\localhost:8088";
var contextOptions = new KSqlDBContextOptions(ksqlDbUrl);
var context = new KSqlDBContext(contextOptions);

context.CreateQueryStream<Tweet>()
  .GroupBy(c => c.Id)
  .Select(g => new { Id = g.Key, Count = g.Count() })
  .Subscribe(count =>
  {
    Console.WriteLine($"{count.Id} Count: {count.Count}");
    Console.WriteLine();
  }, error => { Console.WriteLine($"Exception: {error.Message}"); }, () => Console.WriteLine("Completed"));
```
```SQL
SELECT Id, COUNT(*) Count FROM Tweets GROUP BY Id EMIT CHANGES;
```
Or
```C#
context.CreateQueryStream<Tweet>()
  .GroupBy(c => c.Id)
  .Select(g => g.Count()); 
```
```SQL
SELECT COUNT(*) FROM Tweets GROUP BY Id EMIT CHANGES;
```

#### Sum
```C#
context.CreateQueryStream<Tweet>()
        .GroupBy(c => c.Id)
        //.Select(g => g.Sum(c => c.Amount))
        .Select(g => new { Id = g.Key, Agg = g.Sum(c => c.Amount)})
```
Equivalent to KSql:
```SQL
SELECT Id, SUM(Amount) Agg FROM Tweets GROUP BY Id EMIT CHANGES;
```

### ToAsyncEnumerable (v.0.1.0)
Creates an [async iterator](https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8) from the query:
```C#
var cts = new CancellationTokenSource();
var asyncTweetsEnumerable = context.CreateQueryStream<Tweet>().ToAsyncEnumerable();

await foreach (var tweet in asyncTweetsEnumerable.WithCancellation(cts.Token))
  Console.WriteLine(tweet.Message);
```

### WindowedBy (v.0.1.0)
Creation of windowed aggregation

[Tumbling window](https://docs.ksqldb.io/en/latest/concepts/time-and-windows-in-ksqldb-queries/#tumbling-window):
```C#
var context = new TransactionsDbProvider(TestParameters.KsqlDBUrl);

var windowedQuery = context.CreateQueryStream<Transaction>()
  .WindowedBy(new TimeWindows(Duration.OfSeconds(5)).WithGracePeriod(Duration.OfHours(2)))
  .GroupBy(c => c.CardNumber)
  .Select(g => new { CardNumber = g.Key, Count = g.Count() });
```

```KSQL
SELECT CardNumber, COUNT(*) Count FROM Transactions 
  WINDOW TUMBLING (SIZE 5 SECONDS, GRACE PERIOD 2 HOURS) 
  GROUP BY CardNumber EMIT CHANGES;
```

[Hopping window](https://docs.ksqldb.io/en/latest/concepts/time-and-windows-in-ksqldb-queries/#hopping-window):
```C#

var subscription = context.CreateQueryStream<Tweet>()
  .GroupBy(c => c.Id)
  .WindowedBy(new HoppingWindows(Duration.OfSeconds(5)).WithAdvanceBy(Duration.OfSeconds(4)).WithRetention(Duration.OfDays(7)))
  .Select(g => new { g.WindowStart, g.WindowEnd, Id = g.Key, Count = g.Count() })
  .Subscribe(c => { Console.WriteLine($"{c.Id}: {c.Count}: {c.WindowStart}: {c.WindowEnd}"); }, exception => {});
```

```KSQL
SELECT WindowStart, WindowEnd, Id, COUNT(*) Count FROM Tweets 
  WINDOW HOPPING (SIZE 5 SECONDS, ADVANCE BY 10 SECONDS, RETENTION 7 DAYS) 
  GROUP BY Id EMIT CHANGES;
```
Window advancement interval should be more than zero and less than window duration

### String Functions UCase, LCase (v.0.1.0)
```C#
l => l.Message.ToLower() != "hi";
l => l.Message.ToUpper() != "HI";
```
```KSQL
LCASE(Latitude) != 'hi'
UCASE(Latitude) != 'HI'
```

# v0.2.0 preview
Planned:

### Having (v.0.2.0)
```C#
var query = context.CreateQueryStream<Tweet>()
  .GroupBy(c => c.Id)
  .Having(c => c.Count() > 2)
  .Select(g => new { Id = g.Key, Count = g.Count()});
```
KSQL:
```KSQL
SELECT Id, COUNT(*) Count FROM Tweets GROUP BY Id HAVING Count(*) > 2 EMIT CHANGES;
```

### Session Window (v.0.2.0)
A [session window](https://docs.ksqldb.io/en/latest/concepts/time-and-windows-in-ksqldb-queries/#session-window) aggregates records into a session, which represents a period of activity separated by a specified gap of inactivity, or "idleness". 
```C#
var query = context.CreateQueryStream<Transaction>()
  .GroupBy(c => c.CardNumber)
  .WindowedBy(new SessionWindow(Duration.OfSeconds(5)))
  .Select(g => new { CardNumber = g.Key, Count = g.Count() });
```
KSQL:
```KSQL
SELECT CardNumber, COUNT(*) Count FROM Transactions 
  WINDOW SESSION (5 SECONDS)
  GROUP BY CardNumber 
  EMIT CHANGES;
```

### Avg (v.0.2.0)
```KSQL
AVG(col1)
``` 
Return the average value for a given column.
```C#
var query = CreateQbservable()
  .GroupBy(c => c.RegionCode)
  .Select(g => g.Avg(c => c.Citizens));
```

### Min and Max (v.0.2.0)
```KSQL
MIN(col1)
MAX(col1)
``` 
Return the minimum/maximum value for a given column and window. Rows that have col1 set to null are ignored.
```C#
var queryMin = CreateQbservable()
  .GroupBy(c => c.RegionCode)
  .Select(g => g.Min(c => c.Citizens));
var queryMax = CreateQbservable()
  .GroupBy(c => c.RegionCode)
  .Select(g => g.Max(c => c.Citizens));
```

### Like (v.0.2.0)
```C#
using Kafka.DotNet.ksqlDB.KSql.Query.Functions;

Expression<Func<Tweet, bool>> likeExpression = c => KSql.Functions.Like(c.Message, "%santa%");

Expression<Func<Tweet, bool>> likeLCaseExpression = c => KSql.Functions.Like(c.Message.ToLower(), "%santa%".ToLower());
```
KSQL
```KSQL
"LCASE(Message) LIKE LCASE('%santa%')"
```

### String function - Length (LEN) (v.0.2.0)
```C#
Expression<Func<Tweet, int>> lengthExpression = c => c.Message.Length;
```
KSQL
```KSQL
LEN(Message)
```

**TODO:**
- [aggregation functions](https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-reference/aggregate-functions/) except of count and sum (released in v0.1.0)
- [joining streams and tables](https://docs.ksqldb.io/en/latest/developer-guide/joins/join-streams-and-tables/)
- rest of the [ksql query syntax](https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-reference/select-push-query/) (supported operators etc)
- backpressure support