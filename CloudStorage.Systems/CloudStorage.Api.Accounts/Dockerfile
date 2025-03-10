FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CloudStorage.Systems/CloudStorage.Api.Accounts/CloudStorage.Api.Accounts.csproj", "CloudStorage.Systems/CloudStorage.Api.Accounts/"]
COPY ["CloudStorage.Applications/CloudStorage.Application.Accounts/CloudStorage.Application.Accounts.csproj", "CloudStorage.Applications/CloudStorage.Application.Accounts/"]
COPY ["CloudStorage.Domains/CloudStorage.Domain.Authorization/CloudStorage.Domain.Authorization.csproj", "CloudStorage.Domains/CloudStorage.Domain.Authorization/"]
COPY ["CloudStorage.Domains/CloudStorage.Domain.Core/CloudStorage.Domain.Core.csproj", "CloudStorage.Domains/CloudStorage.Domain.Core/"]
COPY ["CloudStorage.Shared/CloudStorage.Shared.Commons/CloudStorage.Shared.Commons.csproj", "CloudStorage.Shared/CloudStorage.Shared.Commons/"]
COPY ["CloudStorage.Applications/CloudStorage.Application.Tokens/CloudStorage.Application.Tokens.csproj", "CloudStorage.Applications/CloudStorage.Application.Tokens/"]
COPY ["CloudStorage.Applications/CloudStorage.Application.Commons/CloudStorage.Application.Commons.csproj", "CloudStorage.Applications/CloudStorage.Application.Commons/"]
COPY ["CloudStorage.Shared/CloudStorage.Shared.Security/CloudStorage.Shared.Security.csproj", "CloudStorage.Shared/CloudStorage.Shared.Security/"]
RUN dotnet restore "CloudStorage.Systems/CloudStorage.Api.Accounts/CloudStorage.Api.Accounts.csproj"
COPY . .
WORKDIR "/src/CloudStorage.Systems/CloudStorage.Api.Accounts"
RUN dotnet build "CloudStorage.Api.Accounts.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CloudStorage.Api.Accounts.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CloudStorage.Api.Accounts.dll"]
