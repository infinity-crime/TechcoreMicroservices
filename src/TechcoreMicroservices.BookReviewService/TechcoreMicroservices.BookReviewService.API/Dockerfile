# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TechcoreMicroservices.BookReviewService/TechcoreMicroservices.BookReviewService.API/TechcoreMicroservices.BookReviewService.API.csproj", "src/TechcoreMicroservices.BookReviewService/TechcoreMicroservices.BookReviewService.API/"]
COPY ["src/TechcoreMicroservices.BookReviewService/TechcoreMicroservices.BookReviewService.Application/TechcoreMicroservices.BookReviewService.Application.csproj", "src/TechcoreMicroservices.BookReviewService/TechcoreMicroservices.BookReviewService.Application/"]
COPY ["src/TechcoreMicroservices.BookReviewService/TechcoreMicroservices.BookReviewService.Contracts/TechcoreMicroservices.BookReviewService.Contracts.csproj", "src/TechcoreMicroservices.BookReviewService/TechcoreMicroservices.BookReviewService.Contracts/"]
COPY ["src/TechcoreMicroservices.BookReviewService/TechcoreMicroservices.BookReviewService.Domain/TechcoreMicroservices.BookReviewService.Domain.csproj", "src/TechcoreMicroservices.BookReviewService/TechcoreMicroservices.BookReviewService.Domain/"]
COPY ["src/TechcoreMicroservices.BookReviewService/TechcoreMicroservices.BookReviewService.Infrastructure/TechcoreMicroservices.BookReviewService.Infrastructure.csproj", "src/TechcoreMicroservices.BookReviewService/TechcoreMicroservices.BookReviewService.Infrastructure/"]
COPY ["src/TechcoreMicroservices.BookOrderService/TechcoreMicroservices.BookOrderService.Contracts/TechcoreMicroservices.BookOrderService.Contracts.csproj", "src/TechcoreMicroservices.BookOrderService/TechcoreMicroservices.BookOrderService.Contracts/"]
RUN dotnet restore "./src/TechcoreMicroservices.BookReviewService/TechcoreMicroservices.BookReviewService.API/TechcoreMicroservices.BookReviewService.API.csproj"
COPY . .
WORKDIR "/src/src/TechcoreMicroservices.BookReviewService/TechcoreMicroservices.BookReviewService.API"
RUN dotnet build "./TechcoreMicroservices.BookReviewService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TechcoreMicroservices.BookReviewService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechcoreMicroservices.BookReviewService.API.dll"]