# Installation instructions
### Prerequesities:
[.NET 5 preview 4](https://dotnet.microsoft.com/download/dotnet/5.0?utm_source=dotnet-website&utm_medium=banner&utm_campaign=preview5-banner)
[Visual Studio 2019, version 16.7 Preview 1 with ASP.NET and web development](https://docs.microsoft.com/en-us/windows/apps/winui/winui3/get-started-winui3-for-desktop#prerequisites)

### Git
`git clone https://github.com/tomasfabian/Joker.git`

`CD Joker`

Set (multiple) startup project(s) in Joker.sln/Examples:
* Use **docker-compose** for OData server with SqlTableDependencyRedisProvider
* CD Joker\Samples\WinUI3\
* WinUI 3 in Desktop and UWP Preview 1 can be found in Joker\Samples\WinUI3\Joker.WinUI3.Sample.sln.
   1. project "Joker.WinUI3.Desktop (Package)"
   2. project "Joker.WinUI3.UWP.Sample (Universal Windows)"


