# v0.2.0 preview (RC1)

### ExtensionsMethods:
- Having
- Window Session
- Avg - Return the average value for a given column
- Min
- Max
- arithmetic operators
- KSqlFunctions - LIKE
- String functions - LPad, RPad, Trim, Len, Substring
- Numeric scalar functions - Sign, Sqrt
- Inner Join

### Fixes:
- parse single value for anonymous type - KSqlDbQueryStreamProvider bug fix

### TODO:
- https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-reference/scalar-functions/#date-and-time
- string functions: Replace, Regex**
- CreateQueryStream options parameter
- Joins: Left outer, Full outer
- https://docs.ksqldb.io/en/latest/developer-guide/ksqldb-reference/aggregate-functions/#latest_by_offset

# v0.1.0
### ExtensionsMethods:
- AsAsyncEnumerable
- Sum Aggregation 
- Tumbling window, Hopping window

- KSqlDBContext async disposition AsyncDisposableObject
- IKSqlGrouping

- Queries UCASE and LCASE

# v0.1.0-preview3

### Implementations:
- convert query visitor
- Record base type with RowTime property
- KSqlDBContext, KSqlDBContextOptions, QueryContext
- ServiceProvider

### ExtensionsMethods:
- GroupBy
- Count Aggregation  

# v0.1.0-preview2
- KQuerySet was set to internal for maintanance reasons. Is KQueryStreamSet good enough for all push queries?

### ToObservable:
- ToObservable
- Subscribe overloads 

# v0.1.0-preview1
### ksql provider:
- SELECT projections
- FROM entity type (KStream name)
- WHERE conditions (AND, OR)
- EMIT CHANGES
- LIMIT take linq extension method

### ExtensionsMethods:
- Subscribe 
 
### Interfaces:
- IQbservableProvider
- ```IQbservable<TEntity>```

### Implementations:
- KSqlVisitor
- KSqlQueryGenerator - compiler
- KStreamSet, KQuerySet, KQueryStreamSet
- QbservableProvider
- ```KSqldbProvider<T>``` - ksqldb REST api provider for push queries (```KSqlDbQueryProvider<T>```, ```KSqlDbQueryStreamProvider<T>```)