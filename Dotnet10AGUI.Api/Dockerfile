FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Dotnet10AGUI.Api/Dotnet10AGUI.Api.csproj", "Dotnet10AGUI.Api/"]
RUN dotnet restore "Dotnet10AGUI.Api/Dotnet10AGUI.Api.csproj"
COPY . .
WORKDIR "/src/Dotnet10AGUI.Api"
RUN dotnet build "./Dotnet10AGUI.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Dotnet10AGUI.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dotnet10AGUI.Api.dll"]
