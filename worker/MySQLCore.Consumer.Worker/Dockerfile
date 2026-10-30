FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["worker/MySQLCore.Consumer.Worker/MySQLCore.Consumer.Worker.csproj", "worker/MySQLCore.Consumer.Worker/"]
COPY ["worker/MySQLCore.Core.Worker/MySQLCore.Core.Worker.csproj", "worker/MySQLCore.Core.Worker/"]
COPY ["src/MySQLCore.Core/MySQLCore.Core.csproj", "src/MySQLCore.Core/"]
COPY ["src/MySQLCore.Infrastructure/MySQLCore.Infrastructure.csproj", "src/MySQLCore.Infrastructure/"]

RUN dotnet restore "worker/MySQLCore.Consumer.Worker/MySQLCore.Consumer.Worker.csproj"

COPY . .

RUN dotnet publish "worker/MySQLCore.Consumer.Worker/MySQLCore.Consumer.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MySQLCore.Consumer.Worker.dll"]
