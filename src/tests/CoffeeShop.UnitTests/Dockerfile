#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["tests/CoffeeShop.UnitTests/CoffeeShop.UnitTests.csproj", "tests/CoffeeShop.UnitTests/"]
COPY ["Services/Catalog/Catalog.Application/Catalog.Application.csproj", "Services/Catalog/Catalog.Application/"]
COPY ["Common/Foundation/Foundation.csproj", "Common/Foundation/"]
COPY ["Services/Catalog/Catalog.Domain/Catalog.Domain.csproj", "Services/Catalog/Catalog.Domain/"]
COPY ["Services/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "Services/Catalog/Catalog.Infrastructure/"]
COPY ["Common/Caching/RedisCachingService/RedisCachingService.csproj", "Common/Caching/RedisCachingService/"]
RUN dotnet restore "./tests/CoffeeShop.UnitTests/CoffeeShop.UnitTests.csproj"
COPY . .
WORKDIR "/src/tests/CoffeeShop.UnitTests"
RUN dotnet build "./CoffeeShop.UnitTests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CoffeeShop.UnitTests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoffeeShop.UnitTests.dll"]