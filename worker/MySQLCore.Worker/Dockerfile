FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["worker/MySQLCore.Worker/MySQLCore.Worker.csproj", "worker/MySQLCore.Worker/"]
COPY ["src/MySQLCore.Core/MySQLCore.Core.csproj", "src/MySQLCore.Core/"]
COPY ["src/MySQLCore.Infrastructure/MySQLCore.Infrastructure.csproj", "src/MySQLCore.Infrastructure/"]

RUN dotnet restore "worker/MySQLCore.Worker/MySQLCore.Worker.csproj"

COPY . .

RUN dotnet publish "worker/MySQLCore.Worker/MySQLCore.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MySQLCore.Worker.dll"]
