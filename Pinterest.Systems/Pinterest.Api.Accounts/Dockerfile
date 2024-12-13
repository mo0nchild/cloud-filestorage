FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Pinterest.Systems/Pinterest.Api.Accounts/Pinterest.Api.Accounts.csproj", "Pinterest.Systems/Pinterest.Api.Accounts/"]
COPY ["Pinterest.Applications/Pinterest.Application.Accounts/Pinterest.Application.Accounts.csproj", "Pinterest.Applications/Pinterest.Application.Accounts/"]
COPY ["Pinterest.Domains/Pinterest.Domain.Authorization/Pinterest.Domain.Authorization.csproj", "Pinterest.Domains/Pinterest.Domain.Authorization/"]
COPY ["Pinterest.Domains/Pinterest.Domain.Core/Pinterest.Domain.Core.csproj", "Pinterest.Domains/Pinterest.Domain.Core/"]
COPY ["Pinterest.Shared/Pinterest.Shared.Commons/Pinterest.Shared.Commons.csproj", "Pinterest.Shared/Pinterest.Shared.Commons/"]
COPY ["Pinterest.Applications/Pinterest.Application.Tokens/Pinterest.Application.Tokens.csproj", "Pinterest.Applications/Pinterest.Application.Tokens/"]
COPY ["Pinterest.Applications/Pinterest.Application.Commons/Pinterest.Application.Commons.csproj", "Pinterest.Applications/Pinterest.Application.Commons/"]
COPY ["Pinterest.Shared/Pinterest.Shared.Security/Pinterest.Shared.Security.csproj", "Pinterest.Shared/Pinterest.Shared.Security/"]
RUN dotnet restore "Pinterest.Systems/Pinterest.Api.Accounts/Pinterest.Api.Accounts.csproj"
COPY . .
WORKDIR "/src/Pinterest.Systems/Pinterest.Api.Accounts"
RUN dotnet build "Pinterest.Api.Accounts.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Pinterest.Api.Accounts.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pinterest.Api.Accounts.dll"]
