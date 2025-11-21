# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TechcoreMicroservices.AnalyticsWorkerService/AnalyticService/AnalyticService.csproj", "src/TechcoreMicroservices.AnalyticsWorkerService/AnalyticService/"]
COPY ["src/TechcoreMicroservices.BookService/TechcoreMicroservices.BookService.Contracts/TechcoreMicroservices.BookService.Contracts.csproj", "src/TechcoreMicroservices.BookService/TechcoreMicroservices.BookService.Contracts/"]
RUN dotnet restore "./src/TechcoreMicroservices.AnalyticsWorkerService/AnalyticService/AnalyticService.csproj"
COPY . .
WORKDIR "/src/src/TechcoreMicroservices.AnalyticsWorkerService/AnalyticService"
RUN dotnet build "./AnalyticService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AnalyticService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AnalyticService.dll"]