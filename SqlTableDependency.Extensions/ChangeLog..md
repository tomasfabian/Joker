3.0.0
- System.Configuration.ConfigurationManager, System.Reactive and System.ComponentModel.Annotations 5.0

2.3.4
- SqlTableDependencyWithReconnection.Dispose finalizer fix
- removed duplicated code from overridden Stop

2.3.3
- Joker 1.7

2.3.2
- added TableName to SqlTableDependencySettings<TEntity>
- SqlTableDependencyProvider is not abstract anymore

2.3.1
- added support for TableAttribute
- added support for TimeOut and WatchDogTimeOut to SqlTableDependencySettings<TEntity> 

2.3.0
Breaking change:
- SqlTableDependencyProvider.OnUpdated added argument TEntity entityOldValues for settings.IncludeOldValues = true;

2.2.0
- SqlTableDependencyOnError was hidden (set to private, breaking change)
- overriden OnError does not need to call base.OnError
- added reference to Joker library. DisposableObject was removed from SqlTableDependency.Extensions and is taken from Joker.dll.

2.1.0
- added SqlTableDependencySettings for additional SqlTableDependency settings like schemaName, includeOldValues, etc.

2.0.0
- added application scopes