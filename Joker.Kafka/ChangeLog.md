# v0.1.0

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