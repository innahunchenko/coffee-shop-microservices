#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

#COPY ./certs/catalog.api+6.crt /usr/local/share/ca-certificates/catalog.api+6.crt
#RUN apt-get update && apt-get install -y ca-certificates && update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/ShoppingCart/ShoppingCart.API/ShoppingCart.API.csproj", "Services/ShoppingCart/ShoppingCart.API/"]
RUN dotnet restore "./Services/ShoppingCart/ShoppingCart.API/ShoppingCart.API.csproj"
COPY . .
WORKDIR "/src/Services/ShoppingCart/ShoppingCart.API"
RUN dotnet build "./ShoppingCart.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ShoppingCart.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShoppingCart.API.dll"]