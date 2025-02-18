FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Set HTTPS URL binding
ENV ASPNETCORE_URLS="https://+:7840"

# Expose HTTPS port
EXPOSE 7840

# Build and publish application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
ARG project=MySQLCore.API.csproj

WORKDIR /src
COPY ./src .

WORKDIR /src/MySQLCore.API
RUN dotnet restore "${project}"
RUN dotnet build "${project}" -c ${configuration} -o /app/build

# RUN dotnet test

# Publish application
FROM build AS publish
RUN dotnet publish "${project}" -c ${configuration} -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Start the API
ENTRYPOINT [ "dotnet", "MySQLCore.API.dll" ]
