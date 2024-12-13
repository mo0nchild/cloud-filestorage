FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Pinterest.Systems/Pinterest.Api.Posts/Pinterest.Api.Posts.csproj", "Pinterest.Systems/Pinterest.Api.Posts/"]
RUN dotnet restore "Pinterest.Systems/Pinterest.Api.Posts/Pinterest.Api.Posts.csproj"
COPY . .
WORKDIR "/src/Pinterest.Systems/Pinterest.Api.Posts"
RUN dotnet build "Pinterest.Api.Posts.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pinterest.Api.Posts.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pinterest.Api.Posts.dll"]
