FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG PrivateNugetSource
WORKDIR /app

COPY IntegrationFrameworks.Api/IntegrationFrameworks.Api.csproj ./IntegrationFrameworks.Api/IntegrationFrameworks.Api.csproj

RUN dotnet restore IntegrationFrameworks.Api/IntegrationFrameworks.Api.csproj --source ${PrivateNugetSource} --source https://api.nuget.org/v3/index.json

COPY IntegrationFrameworks.Api ./IntegrationFrameworks.Api

RUN dotnet build -c Release IntegrationFrameworks.Api/IntegrationFrameworks.Api.csproj

RUN dotnet publish -c Release -o publish IntegrationFrameworks.Api/IntegrationFrameworks.Api.csproj

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app

COPY --from=build /app/publish .

CMD ["dotnet", "IntegrationFrameworks.Api.dll"]
