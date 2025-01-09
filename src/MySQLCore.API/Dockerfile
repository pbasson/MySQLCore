FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://*:5840

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build 
ARG configuration=Release
ARG project=MySQLCore.API.csproj

WORKDIR /src
COPY ./src .

WORKDIR /src/MySQLCore.API
RUN dotnet restore "${project}"
RUN dotnet build "${project}" -c configuration -o /app/build

FROM build AS publish
RUN dotnet publish "${project}" -c configuration -o /app/publish

FROM base AS final 
WORKDIR /app

EXPOSE 5840

COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "MySQLCore.API.dll" ]