#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Samples/OData/SelfHostedODataService.EFCore/SelfHostedODataService.EFCore.csproj", "Samples/OData/SelfHostedODataService.EFCore/"]
COPY ["Joker.Redis/Joker.Redis.csproj", "Joker.Redis/"]
COPY ["SqlTableDependency.Extensions/SqlTableDependency.Extensions.csproj", "SqlTableDependency.Extensions/"]
COPY ["Joker/Joker.csproj", "Joker/"]
COPY ["Samples/Sample.Data/Sample.Data.csproj", "Samples/Sample.Data/"]
COPY ["Samples/Sample.Domain/Sample.Domain.csproj", "Samples/Sample.Domain/"]
RUN dotnet restore "Samples/OData/SelfHostedODataService.EFCore/SelfHostedODataService.EFCore.csproj"
COPY . .
WORKDIR "/src/Samples/OData/SelfHostedODataService.EFCore"
RUN dotnet build "SelfHostedODataService.EFCore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SelfHostedODataService.EFCore.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SelfHostedODataService.EFCore.dll"]