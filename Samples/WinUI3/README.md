# Installation instructions
### Prerequesities:
[.NET 5 Desktop Runtime 5.0.0](https://dotnet.microsoft.com/download/dotnet/5.0)

[Visual Studio 2019, version 16.9 Preview](https://visualstudio.microsoft.com/cs/vs/preview/)

### Git
`git clone https://github.com/tomasfabian/Joker.git`

`CD Joker`


* Set **docker-compose** as the startup project in Joker.sln and run it for OData server with SqlTableDependencyRedisProvider
* CD Joker\Samples\WinUI3\
* [WinUI 3](https://docs.microsoft.com/en-us/windows/apps/winui/winui3/#install-winui-3-preview-3) in Desktop and UWP Preview 3 can be found in Joker\Samples\WinUI3\Joker.WinUI3.Sample.sln.
   1. project "Joker.WinUI3.Desktop (Package)"
   2. project "Joker.WinUI3.UWP.Sample (Universal Windows)"
