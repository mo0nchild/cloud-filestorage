FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CloudStorage.Systems/CloudStorage.Api.Posts/CloudStorage.Api.Posts.csproj", "CloudStorage.Systems/CloudStorage.Api.Posts/"]
RUN dotnet restore "CloudStorage.Systems/CloudStorage.Api.Posts/CloudStorage.Api.Posts.csproj"
COPY . .
WORKDIR "/src/CloudStorage.Systems/CloudStorage.Api.Posts"
RUN dotnet build "CloudStorage.Api.Posts.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CloudStorage.Api.Posts.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CloudStorage.Api.Posts.dll"]
