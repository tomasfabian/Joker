# v0.1.0-preview3
- convert query visitor
- Record base type with RowTime property
- interception of the stream name in ksql query generation

# v0.1.0-preview2
- added ToObservable extension
- KQuerySet was set to internal for maintanance reasons. Is KQueryStreamSet good enough for all push queries?

# v0.1.0-preview1
### ksql provider:
- SELECT projections
- FROM entity type (KStream name)
- WHERE conditions (AND, OR)
- EMIT CHANGES
- LIMIT take linq extension method

### Interfaces:
- IQbservableProvider
- ```IQbservable<TEntity>```

### Implementations:
- KSqlVisitor
- KSqlQueryGenerator - compiler
- KStreamSet, KQuerySet, KQueryStreamSet
- QbservableProvider
- ```KSqldbProvider<T>``` - ksqldb REST api provider for push queries (```KSqlDbQueryProvider<T>```, ```KSqlDbQueryStreamProvider<T>```)