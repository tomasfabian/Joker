#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Samples/OData/SelfHostedODataService.EFCore/SelfHostedODataService.EFCore.csproj", "Samples/OData/SelfHostedODataService.EFCore/"]
COPY ["Samples/Sample.DataCore/Sample.DataCore.csproj", "Samples/Sample.DataCore/"]
COPY ["Samples/Sample.Domain/Sample.Domain.csproj", "Samples/Sample.Domain/"]
RUN dotnet restore "Samples/OData/SelfHostedODataService.EFCore/SelfHostedODataService.EFCore.csproj"
COPY . .
WORKDIR "/src/Samples/OData/SelfHostedODataService.EFCore"
RUN dotnet build "SelfHostedODataService.EFCore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SelfHostedODataService.EFCore.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SelfHostedODataService.EFCore.dll"]