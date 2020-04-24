FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

LABEL author="Tomas Fabian"
MAINTAINER fabian.frameworks@gmail.com

WORKDIR /publish

COPY . .
RUN dotnet restore ./Samples/OData/SelfHostedODataService/SelfHostedODataService.csproj

RUN dotnet publish -c release -o ./app --no-restore ./Samples/OData/SelfHostedODataService/SelfHostedODataService.csproj

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

#EXPOSE 5000/tcp 
EXPOSE 80/tcp
ENV ASPNETCORE_URLS http://+:80
#ENV ASPNETCORE_URLS http://127.0.0.1:5000

# VOLUME [""]
WORKDIR /app
COPY --from=build /publish/app ./

ENTRYPOINT ["dotnet", "SelfHostedODataService.dll"]
