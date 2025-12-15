FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Dotnet10AISamples.Api/Dotnet10AISamples.Api.csproj", "Dotnet10AISamples.Api/"]
RUN dotnet restore "Dotnet10AISamples.Api/Dotnet10AISamples.Api.csproj"
COPY . .
WORKDIR "/src/Dotnet10AISamples.Api"
RUN dotnet build "./Dotnet10AISamples.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Dotnet10AISamples.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dotnet10AISamples.Api.dll"]
