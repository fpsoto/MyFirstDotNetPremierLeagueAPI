# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY PremierLeague.sln .
COPY src/PremierLeague.Domain/PremierLeague.Domain.csproj src/PremierLeague.Domain/
COPY src/PremierLeague.Application/PremierLeague.Application.csproj src/PremierLeague.Application/
COPY src/PremierLeague.Infrastructure/PremierLeague.Infrastructure.csproj src/PremierLeague.Infrastructure/
COPY src/PremierLeague.Api/PremierLeague.Api.csproj src/PremierLeague.Api/
COPY tests/PremierLeague.UnitTests/PremierLeague.UnitTests.csproj tests/PremierLeague.UnitTests/
COPY tests/PremierLeague.IntegrationTests/PremierLeague.IntegrationTests.csproj tests/PremierLeague.IntegrationTests/

RUN dotnet restore

COPY . .

RUN dotnet build -c Release --no-restore
RUN dotnet publish src/PremierLeague.Api/PremierLeague.Api.csproj -c Release --no-build -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "PremierLeague.Api.dll"]
