﻿This package generates ksql queries from your .NET C# linq queries. You can filter, project, limit etc. your push notifications server side with [ksqlDB push queries](https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-rest-api/streaming-endpoint/)

```
Install-Package Kafka.DotNet.ksqlDB
```
```C#
using System;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.Sample.Model;

var ksqlDbUrl = @"http:\\localhost:8088";

await using var context = new KSqlDBContext(ksqlDbUrl);

using var disposable = context.CreateQueryStream<Tweet>()
  .Where(p => p.Message != "Hello world" || p.Id == 1)
  .Select(l => new { l.Message, l.Id })
  .Take(2)
  .Subscribe(tweetMessage =>
  {
    Console.WriteLine($"{nameof(Tweet)}: {tweetMessage.Id} - {tweetMessage.Message}");
  }, error => { Console.WriteLine($"Exception: {error.Message}"); }, () => Console.WriteLine("Completed"));

Console.WriteLine("Press any key to stop the subscription");

Console.ReadKey();
```

In the above mentioned code snippet everything runs server side except of the ``` IQbservable<TEntity>.Subscribe``` method. It subscribes to your ksqlDB stream created in the following manner:
```SQL
CREATE STREAM tweets(id INT, message VARCHAR)
  WITH (kafka_topic='tweetsTopic', value_format='JSON', partitions=1);
```

LINQ code written in C# from the sample is equivalent to this ksql query:
```SQL
SELECT Message, Id FROM Tweets
WHERE Message != 'Hello world' OR Id = 1 
EMIT CHANGES 
LIMIT 2;
```

Run the following insert statements to stream some messages with your ksqldb-cli
```
docker exec -it $(docker ps -q -f name=ksqldb-cli) ksql http://localhost:8088
```
```SQL
INSERT INTO tweets (id, message) VALUES (1, 'Hello world');
INSERT INTO tweets (id, message) VALUES (2, 'ksqlDB rulez!');
```
Sample project can be found under [Examples/Kafka](https://github.com/tomasfabian/Joker/tree/master/Samples/Kafka/Kafka.DotNet.ksqlDB.Sample) solution folder in Joker.sln or Joker.DotNet5.sln 



**External dependencies:**
- [kafka broker](https://kafka.apache.org/intro) and [ksqlDB-server](https://ksqldb.io/overview.html) 0.14.0


CD to [Examples/Kafka](https://github.com/tomasfabian/Joker/tree/master/Samples/Kafka/Kafka.DotNet.ksqlDB.Sample)

run in command line:

```docker compose up -d```

# Setting query parameters (v0.1.0)
Default settings:
'auto.offset.reset' is set to 'earliest' by default. 
New parameters could be added or existing ones changed in the following manner:
```C#
var contextOptions = new KSqlDBContextOptions(@"http:\\localhost:8088");

contextOptions.QueryStreamParameters["auto.offset.reset"] = "latest";
```

### Record (row) class (v0.1.0)
Record class is a base class for rows returned in push queries. It has a 'RowTime' property.

```C#
public class Tweet : Kafka.DotNet.ksqlDB.KSql.Query.Record
{
  public string Message { get; set; }
}

context.CreateQueryStream<Tweet>()
  .Select(c => new { c.RowTime, c.Message });
```

### Overriding stream name (v0.1.0)
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
<img src="https://sec.ch9.ms/ecn/content/images/WhatHowWhere.jpg" />

### Select (v0.1.0)
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
| ```ARRAY<ElementType>``` | C#Type[]   |
| ```MAP<ElementType, ElementType>``` | IDictionary<C#Type, C#Type>   |
| ```STRUCT``` | struct   |

Array type mapping example (available from v0.3.0):
All of the elements in the array must be of the same type. The element type can be any valid SQL type.
```
ksql: ARRAY<INTEGER>
C#  : int[]
```
Destructuring an array (ksqldb represents the first element of an array as 1):
```C#
queryStream
  .Select(_ => new { FirstItem = new[] {1, 2, 3}[1] })
```
Generates the following KSQL:
```KSQL
ARRAY[1, 2, 3][1] AS FirstItem
```
Array length:
```C#
queryStream
  .Select(_ => new[] {1, 2, 3}.Length)
```
Generates the following KSQL:
```KSQL
ARRAY_LENGTH(ARRAY[1, 2, 3])
```

Struct type mapping example (available from v0.5.0):
A struct represents strongly typed structured data. A struct is an ordered collection of named fields that have a specific type. The field types can be any valid SQL type.
```C#
struct Point
{
  public int X { get; set; }

  public int Y { get; set; }
}

queryStream
  .Select(c => new Point { X = c.X, Y = 2 });
```
Generates the following KSQL:
```KSQL
SELECT STRUCT(X := X, Y := 2) FROM StreamName EMIT CHANGES;
```

Destructure a struct:
```C#
queryStream
  .Select(c => new Point { X = c.X, Y = 2 }.X);
```
```KSQL
SELECT STRUCT(X := X, Y := 2)->X FROM StreamName EMIT CHANGES;
```

### Where (v0.1.0)
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
| NOT      | logical NOT                 |  !   |

### Take (Limit) (v0.1.0)
Returns a specified number of contiguous elements from the start of a stream. Depends on the 'auto.topic.offset' parameter.
```C#
context.CreateQueryStream<Tweet>()
  .Take(2);
```
```SQL
SELECT * from tweets EMIT CHANGES LIMIT 2;
```

### Subscribe (v0.1.0)
Providing ```IObserver<T>```:
```C#
using var subscription = new KSqlDBContext(@"http:\\localhost:8088")
  .CreateQueryStream<Tweet>()
  .Subscribe(new TweetsObserver());

public class TweetsObserver : System.IObserver<Tweet>
{
  public void OnNext(Tweet tweetMessage)
  {
    Console.WriteLine($"{nameof(Tweet)}: {tweetMessage.Id} - {tweetMessage.Message}");
  }

  public void OnError(Exception error)
  {
    Console.WriteLine($"{nameof(Tweet)}: {error.Message}");
  }

  public void OnCompleted()
  {
    Console.WriteLine($"{nameof(Tweet)}: completed successfully");
  }
}
```
Providing ```Action<T> onNext, Action<Exception> onError and Action onCompleted```:
```C#
using var subscription = new KSqlDBContext(@"http:\\localhost:8088")
    .CreateQueryStream<Tweet>()
    .Subscribe(
      onNext: tweetMessage =>
      {
        Console.WriteLine($"{nameof(Tweet)}: {tweetMessage.Id} - {tweetMessage.Message}");
      },
      onError: error => { Console.WriteLine($"Exception: {error.Message}"); }, 
      onCompleted: () => Console.WriteLine("Completed")
      );
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
KSql.Linq.IQbservable<Tweet> queryStream = context.CreateQueryStream<Tweet>().Take(2);

System.IObservable<Tweet> observable = queryStream.ToObservable();

//All reactive extension methods are available from this point.
//The not obvious difference is that the processing is done client side, not server side (ksqldb) as in the case of IQbservable:
observable.Throttle(TimeSpan.FromSeconds(3)).Subscribe();
```
WPF client side batching example:
```C#
private static IDisposable ClientSideBatching(KSqlDBContext context)
{
  var disposable = context.CreateQueryStream<Tweet>()
    //Client side execution
    .ToObservable()
    .Buffer(TimeSpan.FromMilliseconds(250), 100)
    .Where(c => c.Count > 0)
    .ObserveOn(System.Reactive.Concurrency.DispatcherScheduler.Current)
    .Subscribe(tweets =>
    {
      foreach (var tweet in tweets)
      {
        Console.WriteLine(tweet.Message);
      }
    });

  return disposable;
}
```

### ToQueryString (v0.1.0)
ToQueryString is helpful for debugging purposes. It returns the generated ksql query without executing it.
```C#
var ksql = context.CreateQueryStream<Tweet>().ToQueryString();

//prints SELECT * FROM tweets EMIT CHANGES;
Console.WriteLine(ksql);
```

### GroupBy (v0.1.0)
#### Count (v0.1.0)
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
` `
> ⚠ There is a known limitation in the early access versions (bellow 1.0). The aggregation functions have to be named/aliased COUNT(*) Count, otherwise the deserialization won't be able to map the unknown column name KSQL_COL_0. 
The Key should be mapped back to the respective column too Id = g.Key

Or without the new expression:
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

### ToAsyncEnumerable (v0.1.0)
Creates an [async iterator](https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8) from the query:
```C#
var cts = new CancellationTokenSource();
var asyncTweetsEnumerable = context.CreateQueryStream<Tweet>().ToAsyncEnumerable();

await foreach (var tweet in asyncTweetsEnumerable.WithCancellation(cts.Token))
  Console.WriteLine(tweet.Message);
```

### WindowedBy (v0.1.0)
Creation of windowed aggregation

[Tumbling window](https://docs.ksqldb.io/en/latest/concepts/time-and-windows-in-ksqldb-queries/#tumbling-window):
```C#
var context = new TransactionsDbProvider(ksqlDbUrl);

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

### String Functions UCase, LCase (v0.1.0)
```C#
l => l.Message.ToLower() != "hi";
l => l.Message.ToUpper() != "HI";
```
```KSQL
LCASE(Latitude) != 'hi'
UCASE(Latitude) != 'HI'
```

# v0.2.0
```
Install-Package Kafka.DotNet.ksqlDB -Version 0.2.0
```

### Having (v0.2.0)
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

### Session Window (v0.2.0)
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
Time units:
```C#
using Kafka.DotNet.ksqlDB.KSql.Query.Windows;

public enum TimeUnits
{
  MILLISECONDS, // v2.0.0
  SECONDS,
  MINUTES,
  HOURS,
  DAYS
}

Duration duration = Duration.OfHours(2);

Console.WriteLine($"{duration.Value} {duration.TimeUnit}");
```

### Inner Joins (v0.2.0)
How to [join table and table](https://kafka-tutorials.confluent.io/join-a-table-to-a-table/ksql.html)
```C#
public class Movie : Record
{
  public string Title { get; set; }
  public int Id { get; set; }
  public int Release_Year { get; set; }
}

public class Lead_Actor : Record
{
  public string Title { get; set; }
  public string Actor_Name { get; set; }
}

using Kafka.DotNet.ksqlDB.KSql.Linq;

var query = context.CreateQueryStream<Movie>()
  .Join(
    Source.Of<Lead_Actor>(nameof(Lead_Actor)),
    movie => movie.Title,
    actor => actor.Title,
    (movie, actor) => new
    {
      movie.Id,
      Title = movie.Title,
      movie.Release_Year,
      ActorName = K.Functions.RPad(K.Functions.LPad(actor.Actor_Name.ToUpper(), 15, "*"), 25, "^"),
      ActorTitle = actor.Title
    }
  );

var joinQueryString = query.ToQueryString();
```
KSQL:
```KSQL
SELECT M.Id Id, M.Title Title, M.Release_Year Release_Year, RPAD(LPAD(UCASE(L.Actor_Name), 15, '*'), 25, '^') ActorName, L.Title ActorTitle 
FROM Movies M
INNER JOIN Lead_Actor L
ON M.Title = L.Title
EMIT CHANGES;
```
` `
> ⚠ There is a known limitation in the early access versions (bellow 1.0). 
The Key column, in this case movie.Title, has to be aliased Title = movie.Title, otherwise the deserialization won't be able to map the unknown column name M_TITLE. 

### Avg (v0.2.0)
```KSQL
AVG(col1)
``` 
Return the average value for a given column.
```C#
var query = CreateQbservable()
  .GroupBy(c => c.RegionCode)
  .Select(g => g.Avg(c => c.Citizens));
```

### Aggregation functions Min and Max (v0.2.0)
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

### Like (v0.2.0)
```C#
using Kafka.DotNet.ksqlDB.KSql.Query.Functions;

Expression<Func<Tweet, bool>> likeExpression = c => KSql.Functions.Like(c.Message, "%santa%");

Expression<Func<Tweet, bool>> likeLCaseExpression = c => KSql.Functions.Like(c.Message.ToLower(), "%santa%".ToLower());
```
KSQL
```KSQL
"LCASE(Message) LIKE LCASE('%santa%')"
```

### Arithmetic operations on columns (v0.2.0)
The usual arithmetic operators (+,-,/,*,%) may be applied to numeric types, like INT, BIGINT, and DOUBLE:
```KSQL
SELECT USERID, LEN(FIRST_NAME) + LEN(LAST_NAME) AS NAME_LENGTH FROM USERS EMIT CHANGES;
```
```C#
Expression<Func<Person, object>> expression = c => c.FirstName.Length * c.LastName.Length;
```

### String function - Length (LEN) (v0.2.0)
```C#
Expression<Func<Tweet, int>> lengthExpression = c => c.Message.Length;
```
KSQL
```KSQL
LEN(Message)
```

### LPad, RPad, Trim, Substring (v0.2.0)
```C#
using Kafka.DotNet.ksqlDB.KSql.Query.Functions;

Expression<Func<Tweet, string>> expression1 = c => KSql.Functions.LPad(c.Message, 8, "x");
Expression<Func<Tweet, string>> expression2 = c => KSql.Functions.RPad(c.Message, 8, "x");
Expression<Func<Tweet, string>> expression3 = c => KSql.Functions.Trim(c.Message);
Expression<Func<Tweet, string>> expression4 = c => K.Functions.Substring(c.Message, 2, 3);
```
KSQL
```KSQL
LPAD(Message, 8, 'x')
RPAD(Message, 8, 'x')
TRIM(Message)
Substring(Message, 2, 3)
```

# v0.3.0
```
Install-Package Kafka.DotNet.ksqlDB -Version 0.3.0
```
## Aggregation functions 
### EarliestByOffset, LatestByOffset, EarliestByOffsetAllowNulls, LatestByOffsetAllowNull (v0.3.0)
[EarliestByOffset](https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-reference/aggregate-functions/#earliest_by_offset),
[LatestByOffset](https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-reference/aggregate-functions/#latest_by_offset)
```C#
Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression1 = l => new { EarliestByOffset = l.EarliestByOffset(c => c.Amount) };

Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression2 = l => new { LatestByOffsetAllowNulls = l.LatestByOffsetAllowNulls(c => c.Amount) };
```
KSQL
```KSQL
--EARLIEST_BY_OFFSET(col1, [ignoreNulls])
EARLIEST_BY_OFFSET(Amount, True) EarliestByOffset
LATEST_BY_OFFSET(Amount, False) LatestByOffsetAllowNulls
```

EARLIEST_BY_OFFSET(col1, earliestN, [ignoreNulls])

Return the earliest N values for the specified column as an ARRAY. The earliest values
in the partition have the lowest offsets.
```C#
await using var context = new KSqlDBContext(@"http:\\localhost:8088");

context.CreateQueryStream<Tweet>()
  .GroupBy(c => c.Id)
  .Select(g => new { Id = g.Key, EarliestByOffset = g.EarliestByOffset(c => c.Amount, 2) })
  .Subscribe(earliest =>
  {
    Console.WriteLine($"{earliest.Id} array length: {earliest.EarliestByOffset.Length}");
  }, error => { Console.WriteLine($"Exception: {error.Message}"); }, () => Console.WriteLine("Completed"));
```
Generated KSQL:
```KSQL
SELECT Id, EARLIEST_BY_OFFSET(Amount, 2, True) EarliestByOffset 
FROM Tweets GROUP BY Id EMIT CHANGES;
```

### TopK, TopKDistinct, LongCount, Count(column) (v0.3.0)
```C#
Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression1 = l => new { TopK = l.TopK(c => c.Amount, 2) };
Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression2 = l => new { TopKDistinct = l.TopKDistinct(c => c.Amount, 2) };
Expression<Func<IKSqlGrouping<int, Transaction>, object>> expression3 = l => new { Count = l.LongCount(c => c.Amount) };
```
KSQL
```KSQL
TOPK(Amount, 2) TopKDistinct
TOPKDISTINCT(Amount, 2) TopKDistinct
COUNT(Amount) Count
```

```C#
new KSqlDBContext(@"http:\\localhost:8088").CreateQueryStream<Tweet>()
  .GroupBy(c => c.Id)
  .Select(g => new { Id = g.Key, TopK = g.TopKDistinct(c => c.Amount, 4) })
  .Subscribe(onNext: tweetMessage =>
  {
    var tops = string.Join(',', tweetMessage.TopK);
    Console.WriteLine($"TopKs: {tops}");
    Console.WriteLine($"TopKs Array Length: {tops.Length}");
  }, onError: error => { Console.WriteLine($"Exception: {error.Message}"); }, onCompleted: () => Console.WriteLine("Completed"));
```

### LeftJoin - LEFT OUTER (v0.3.0)
LEFT OUTER joins will contain leftRecord-NULL records in the result stream, which means that the join contains NULL values for fields selected from the right-hand stream where no match is made.
```C#
var query = new KSqlDBContext(@"http:\\localhost:8088").CreateQueryStream<Movie>()
  .LeftJoin(
    Source.Of<Lead_Actor>(),
    movie => movie.Title,
    actor => actor.Title,
    (movie, actor) => new
    {
      movie.Id,
      ActorTitle = actor.Title
    }
  );
```
Generated KSQL:
```KSQL
SELECT M.Id Id, L.Title ActorTitle FROM Movies M
LEFT JOIN Lead_Actors L
ON M.Title = L.Title
EMIT CHANGES;
```

### Having - aggregations with column (v0.3.0)
[Example](https://kafka-tutorials.confluent.io/finding-distinct-events/ksql.html) shows how to use Having with Count(column) and Group By compound key:
```C#
public class Click
{
  public string IP_ADDRESS { get; set; }
  public string URL { get; set; }
  public string TIMESTAMP { get; set; }
}

var query = context.CreateQueryStream<Click>()
  .GroupBy(c => new { c.IP_ADDRESS, c.URL, c.TIMESTAMP })
  .WindowedBy(new TimeWindows(Duration.OfMinutes(2)))
  .Having(c => c.Count(g => c.Key.IP_ADDRESS) == 1)
  .Select(g => new { g.Key.IP_ADDRESS, g.Key.URL, g.Key.TIMESTAMP })
  .Take(3);
```
Generated KSQL:
```KSQL
SELECT IP_ADDRESS, URL, TIMESTAMP FROM Clicks WINDOW TUMBLING (SIZE 2 MINUTES) GROUP BY IP_ADDRESS, URL, TIMESTAMP 
HAVING COUNT(IP_ADDRESS) = 1 EMIT CHANGES LIMIT 3;
```

### Where IS NULL, IS NOT NULL (v0.3.0)
```C#
using var subscription = new KSqlDBContext(@"http:\\localhost:8088")
  .CreateQueryStream<Click>()
  .Where(c => c.IP_ADDRESS != null || c.IP_ADDRESS == null)
  .Select(c => new { c.IP_ADDRESS, c.URL, c.TIMESTAMP });
```

Generated KSQL:
```KSQL
SELECT IP_ADDRESS, URL, TIMESTAMP
FROM Clicks
WHERE IP_ADDRESS IS NOT NULL OR IP_ADDRESS IS NULL
EMIT CHANGES;
```

### Numeric functions - Abs, Ceil, Floor, Random, Sign, Round (v0.3.0)
```C#
Expression<Func<Tweet, double>> expression1 = c => K.Functions.Abs(c.Amount);
Expression<Func<Tweet, double>> expression2 = c => K.Functions.Ceil(c.Amount);
Expression<Func<Tweet, double>> expression3 = c => K.Functions.Floor(c.Amount);
Expression<Func<Tweet, double>> expression4 = c => K.Functions.Random();
Expression<Func<Tweet, double>> expression5 = c => K.Functions.Sign(c.Amount);

int scale = 3;
Expression<Func<Tweet, double>> expression6 = c => K.Functions.Round(c.Amount, scale);
```

Generated KSQL:
```KSQL
ABS(Amount)
CEIL(AccountBalance)
FLOOR(AccountBalance)
RANDOM()
SIGN(Amount)

ROUND(Amount, 3)
```

### Dynamic - calling not supported ksqldb functions (v0.3.0)
Some of the ksqldb functions have not been implemented yet. This can be circumvented by calling K.Functions.Dynamic with the appropriate function call and its paramaters. The type of the column value is set with C# **as** operator.
```C#
using Kafka.DotNet.ksqlDB.KSql.Query.Functions;

context.CreateQueryStream<Tweet>()
  .Select(c => new { Col = KSql.Functions.Dynamic("IFNULL(Message, 'n/a')") as string, c.Id, c.Amount, c.Message });
```
The interesting part from the above query is:
```C#
K.Functions.Dynamic("IFNULL(Message, 'n/a')") as string
```
Generated KSQL:
```KSQL
SELECT IFNULL(Message, 'n/a') Col, Id, Amount, Message FROM Tweets EMIT CHANGES;
```
Result:
```
+----------------------------+----------------------------+----------------------------+----------------------------+
|COL                         |ID                          |AMOUNT                      |MESSAGE                     |
+----------------------------+----------------------------+----------------------------+----------------------------+
|Hello world                 |1                           |0.0031                      |Hello world                 |
|n/a                         |1                           |0.1                         |null                        |
```

Dynamic function call with array result example:
```C#
using K = Kafka.DotNet.ksqlDB.KSql.Query.Functions.KSql;

context.CreateQueryStream<Tweet>()
  .Select(c => K.F.Dynamic("ARRAY_DISTINCT(ARRAY[1, 1, 2, 3, 1, 2])") as int[])
  .Subscribe(
    message => Console.WriteLine($"{message[0]} - {message[^1]}"), 
    error => Console.WriteLine($"Exception: {error.Message}"));
```

### Aggregation functions: CollectSet, CollectList, CountDistinct (v0.3.0)
```C#
var subscription = context.CreateQueryStream<Tweet>()
  .GroupBy(c => c.Id)
  .Select(g => new { Id = g.Key, Array = g.CollectSet(c => c.Message) })
  //.Select(g => new { Id = g.Key, Array = g.CollectList(c => c.Message) })
  .Subscribe(c =>
  {
    Console.WriteLine($"{c.Id}:");
    foreach (var value in c.Array)
    {
      Console.WriteLine($"  {value}");
    }
  }, exception => { Console.WriteLine(exception.Message); });
```
Generated KSQL:
```KSQL
SELECT Id, COLLECT_SET(Message) Array 
FROM Tweets GROUP BY Id EMIT CHANGES;

SELECT Id, COLLECT_LIST(Message) Array 
FROM Tweets GROUP BY Id EMIT CHANGES;
```

CountDistinct, LongCountDistinct
```C#
var subscription = context.CreateQueryStream<Tweet>()
  .GroupBy(c => c.Id)
  // .Select(g => new { Id = g.Key, Count = g.CountDistinct(c => c.Message) })
  .Select(g => new { Id = g.Key, Count = g.LongCountDistinct(c => c.Message) })
  .Subscribe(c =>
  {
    Console.WriteLine($"{c.Id} - {c.Count}");
  }, exception => { Console.WriteLine(exception.Message); });
```
Generated KSQL:
```KSQL
SELECT Id, COUNT_DISTINCT(Message) Count 
FROM Tweets GROUP BY Id EMIT CHANGES;
```

# v0.4.0
```
Install-Package Kafka.DotNet.ksqlDB -Version 0.4.0
```
### Maps (v0.4.0)
[Maps](https://docs.ksqldb.io/en/latest/how-to-guides/query-structured-data/#maps)
are an associative data type that map keys of any type to values of any type. The types across all keys must be the same. The same rule holds for values. Destructure maps using bracket syntax ([]).
```C#
var dictionary = new Dictionary<string, int>()
{
  { "c", 2 },
  { "d", 4 }
};
``` 
```KSQL
MAP('c' := 2, 'd' := 4)
```

Accessing map elements:
```C#
dictionary["c"]
``` 
```KSQL
MAP('c' := 2, 'd' := 4)['d'] 
```
Deeply nested types:
```C#
context.CreateQueryStream<Tweet>()
  .Select(c => new
  {
    Map = new Dictionary<string, int[]>
    {
      { "a", new[] { 1, 2 } },
      { "b", new[] { 3, 4 } },
    }
  });
```
Generated KSQL:
```KSQL
SELECT MAP('a' := ARRAY[1, 2], 'b' := ARRAY[3, 4]) Map 
FROM Tweets EMIT CHANGES;
```

### Date and time functions
#### DATETOSTRING (v0.4.0)
```C#
int epochDays = 18672;
string format = "yyyy-MM-dd";

Expression<Func<Tweet, string>> expression = _ => KSqlFunctions.Instance.DateToString(epochDays, format);
```
Generated KSQL:
```KSQL
DATETOSTRING(18672, 'yyyy-MM-dd')
```

#### TIMESTAMPTOSTRING (v0.4.0)
```C#
new KSqlDBContext(ksqlDbUrl).CreateQueryStream<Movie>()
  .Select(c => K.Functions.TimestampToString(c.RowTime, "yyyy-MM-dd''T''HH:mm:ssX"))
```

Generated KSQL:
```KSQL
SELECT DATETOSTRING(1613503749145, 'yyyy-MM-dd''T''HH:mm:ssX')
FROM tweets EMIT CHANGES;
```

#### date and time scalar functions (v0.4.0)
[Date and time](https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-reference/scalar-functions/#date-and-time)


# v0.5.0
```
Install-Package Kafka.DotNet.ksqlDB -Version 0.5.0
```

### Structs (v0.5.0)
[Structs](https://docs.ksqldb.io/en/latest/how-to-guides/query-structured-data/#structs)
 are an associative data type that map VARCHAR keys to values of any type. Destructure structs by using arrow syntax (->).

### Entries (v0.5.0)
```C#
bool sorted = true;
      
var subscription = new KSqlDBContext(@"http:\\localhost:8088")
  .CreateQueryStream<Movie>()
  .Select(c => new
  {
    Entries = KSqlFunctions.Instance.Entries(new Dictionary<string, string>()
    {
      {"a", "value"}
    }, sorted)
  })
  .Subscribe(c =>
  {
    foreach (var entry in c.Entries)
    {
      var key = entry.K;

      var value = entry.V;
    }
  }, error => {});
```

Generated KSQL:
```KSQL
SELECT ENTRIES(MAP('a' := 'value'), True) Entries 
FROM movies_test EMIT CHANGES;
```

### Full Outer Join (v0.5.0)
FULL OUTER joins will contain leftRecord-NULL or NULL-rightRecord records in the result stream, which means that the join contains NULL values for fields coming from a stream where no match is made.
Define nullable primitive value types in POCOs:
```C#
public record Movie
{
  public long RowTime { get; set; }
  public string Title { get; set; }
  public int? Id { get; set; }
  public int? Release_Year { get; set; }
}

public class Lead_Actor
{
  public string Title { get; set; }
  public string Actor_Name { get; set; }
}
```

```C#
var source = new KSqlDBContext(@"http:\\localhost:8088")
  .CreateQueryStream<Movie>()
  .FullOuterJoin(
    Source.Of<Lead_Actor>("Actors"),
    movie => movie.Title,
    actor => actor.Title,
    (movie, actor) => new
    {
      movie.Id,
      Title = movie.Title,
      movie.Release_Year,
      ActorTitle = actor.Title
    }
  );
```

Generated KSQL:
```KSQL
SELECT m.Id Id, m.Title Title, m.Release_Year Release_Year, l.Title ActorTitle FROM movies_test m
FULL OUTER JOIN lead_actor_test l
ON m.Title = l.Title
EMIT CHANGES;
```

# v0.6.0:
### CASE (v0.6.0)
- Select a condition from one or more expressions.
```C#
var query = new KSqlDBContext(@"http:\\localhost:8088")
  .CreateQueryStream<Tweet>()
  .Select(c =>
    new
    {
      case_result =
        (c.Amount < 2.0) ? "small" :
        (c.Amount < 4.1) ? "medium" : "large"
    }
  );
```

```KSQL
SELECT 
  CASE 
    WHEN Amount < 2 THEN 'small' 
    WHEN Amount < 4.1 THEN 'medium' 
    ELSE 'large' 
  END AS case_result 
FROM Tweets EMIT CHANGES;
```

**NOTE:** Switch expressions and if-elseif-else statements are not supported at current versions

### KSqlDbContextOptionsBuilder (v0.6.0)
```C#
public static KSqlDBContextOptions CreateQueryStreamOptions(string ksqlDbUrl)
{
  var contextOptions = new KSqlDbContextOptionsBuilder()
    .UseKSqlDb(ksqlDbUrl)
    .SetupQueryStream(options =>
    {
    })
    .SetupQuery(options =>
    {
      options.Properties[QueryParameters.AutoOffsetResetPropertyName] = AutoOffsetReset.Latest.ToString().ToLower();
    })
    .Options;

  return contextOptions;
}
```

# TFM netstandard 2.0 (.Net Framework, NetCoreApp 2.0 etc.) (v0.6.0)
netstandard 2.0 does not support Http 2.0. Due to this ```IKSqlDBContext.CreateQueryStream<TEntity>``` is not exposed at the current version. 
For these reasons ```IKSqlDBContext.CreateQuery<TEntity>``` was introduced to provide the same functionality via Http 1.1. 

### CreateQueryStream (v0.1.0)
[Executing pull or push queries](https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-rest-api/streaming-endpoint/#executing-pull-or-push-queries)
```JSON
POST /query-stream HTTP/2.0
Accept: application/vnd.ksqlapi.delimited.v1
Content-Type: application/vnd.ksqlapi.delimited.v1

{
  "sql": "SELECT * FROM movies EMIT CHANGES;",
  "properties": {
    "auto.offset.reset": "earliest"
  }
}
```
```C#
using System;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.Sample.Models.Movies;

var ksqlDbUrl = @"http:\\localhost:8088";
var contextOptions = CreateQueryStreamOptions(ksqlDbUrl);

await using var context = new KSqlDBContext(contextOptions);

using var disposable = context.CreateQueryStream<Movie>()        
  .Subscribe(onNext: movie =>
  {
    Console.WriteLine($"{nameof(Movie)}: {movie.Id} - {movie.Title} - {movie.RowTime}");
    Console.WriteLine();
  }, onError: error => { Console.WriteLine($"Exception: {error.Message}"); }, onCompleted: () => Console.WriteLine("Completed"));
```

### CreateQuery (v0.6.0)
[Run a query](https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-rest-api/query-endpoint/#post-query)
```JSON
POST /query HTTP/1.1
Accept: application/vnd.ksql.v1+json
Content-Type: application/vnd.ksql.v1+json

{
  "ksql": "SELECT * FROM movies EMIT CHANGES;",
  "streamsProperties": {
    "ksql.streams.auto.offset.reset": "earliest"
  }
}
```
```C#
using System;
using Kafka.DotNet.ksqlDB.KSql.Linq;
using Kafka.DotNet.ksqlDB.KSql.Query.Context;
using Kafka.DotNet.ksqlDB.Sample.Models.Movies;

var ksqlDbUrl = @"http:\\localhost:8088";
var contextOptions = CreateQueryStreamOptions(ksqlDbUrl);

await using var context = new KSqlDBContext(contextOptions);

using var disposable = context.CreateQuery<Movie>()        
  .Subscribe(onNext: movie =>
  {
    Console.WriteLine($"{nameof(Movie)}: {movie.Id} - {movie.Title} - {movie.RowTime}");
    Console.WriteLine();
  }, onError: error => { Console.WriteLine($"Exception: {error.Message}"); }, onCompleted: () => Console.WriteLine("Completed"));
```

# v0.7.0-rc.1 (WIP):
```
Install-Package Kafka.DotNet.ksqlDB -Version 0.7.0-rc.1
```

- scalar collection functions: ArrayIntersect, ArrayJoin

### Lexical precedence (v0.7.0)
You can use parentheses to change the order of evaluation:
```C#
await using var context = new KSqlDBContext(@"http:\\localhost:8088");

var query = context.CreateQueryStream<Location>()
  .Select(c => (c.Longitude + c.Longitude) * c.Longitude);
```

```KSQL
SELECT (Longitude + Longitude) * Longitude FROM Locations EMIT CHANGES;
```

In Where clauses:
```C#
await using var context = new KSqlDBContext(@"http:\\localhost:8088");

var query = context.CreateQueryStream<Location>()
  .Where(c => (c.Latitude == "1" || c.Latitude != "2") && c.Latitude == "3");
```

```KSQL
SELECT * FROM Locations
WHERE ((Latitude = '1') OR (Latitude != '2')) AND (Latitude = '3') EMIT CHANGES;
```

Redundant brackets are not reduced in the current version

### Raw string KSQL query execution (v0.7.0)
The following examples show how to execute ksql queries from strings:
```C#
string ksql = @"SELECT * FROM Movies
WHERE Title != 'E.T.' EMIT CHANGES LIMIT 2;";

QueryParameters queryParameters = new QueryParameters
{
  Sql = ksql,
  [QueryParameters.AutoOffsetResetPropertyName] = "earliest",
};

await using var context = new KSqlDBContext(@"http:\\localhost:8088");

var moviesSource = context.CreateQuery<Movie>(queryParameters)
  .ToObservable();
```

Query stream:
```C#
string ksql = @"SELECT * FROM Movies
WHERE Title != 'E.T.' EMIT CHANGES LIMIT 2;";

QueryStreamParameters queryStreamParameters = new QueryStreamParameters
{
  Sql = ksql,
  [QueryStreamParameters.AutoOffsetResetPropertyName] = "earliest",
};

await using var context = new KSqlDBContext(@"http:\\localhost:8088");

var source = context.CreateQueryStream<Movie>(queryStreamParameters)
  .ToObservable();
```

# Nuget
https://www.nuget.org/packages/Kafka.DotNet.ksqlDB/

**TODO:**
- missing [aggregation functions](https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-reference/aggregate-functions/) and [scalar functions](https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-reference/scalar-functions/)
- rest of the [ksql query syntax](https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-reference/select-push-query/) (supported operators etc)
- backpressure support

# ksqldb links
[Scalar functions](https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-reference/scalar-functions/#as_value)

[Aggregation functions](https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-reference/aggregate-functions/)

[Push query](https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-reference/select-push-query/)