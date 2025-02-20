FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Pinterest.Systems/Pinterest.Worker.SecretsStorage/Pinterest.Worker.SecretsStorage.csproj", "Pinterest.Systems/Pinterest.Worker.SecretsStorage/"]
RUN dotnet restore "Pinterest.Systems/Pinterest.Worker.SecretsStorage/Pinterest.Worker.SecretsStorage.csproj"
COPY . .
WORKDIR "/src/Pinterest.Systems/Pinterest.Worker.SecretsStorage"
RUN dotnet build "Pinterest.Worker.SecretsStorage.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pinterest.Worker.SecretsStorage.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pinterest.Worker.SecretsStorage.dll"]
