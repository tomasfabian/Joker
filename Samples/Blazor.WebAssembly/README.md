# Installation instructions
### Prerequesities:
[.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)
[Visual Studio 2019 16.6 with ASP.NET and web development](https://visualstudio.microsoft.com/cs/thank-you-downloading-visual-studio/?sku=community)

### Git
`git clone https://github.com/tomasfabian/Joker.git`

`CD Joker`

Set **docker-compose** as the startup project in Joker.sln and run it for OData server with SqlTableDependencyRedisProvider

Blazor WebAssembly client with SignalR pubsub from Sql server. This project depends on running [OData server project](https://github.com/tomasfabian/Joker/tree/master/Samples/OData/SelfHostedODataService)

#Work in progress
- Redis ISubscriber does not support WASM platform. SignalR is used instead of it.
