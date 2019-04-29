FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["FindLostThingsBackEnd/FindLostThingsBackEnd.csproj", "FindLostThingsBackEnd/"]
COPY . .
WORKDIR "/src/FindLostThingsBackEnd"

FROM build AS publish
RUN dotnet publish "FindLostThingsBackEnd.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FindLostThingsBackEnd.dll"]