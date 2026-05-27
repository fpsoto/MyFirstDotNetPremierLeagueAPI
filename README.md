# Premier League Stats API

A professional, production-ready ASP.NET Core 9 Web API demonstrating **Clean Architecture**, **CQRS with MediatR**, **Entity Framework Core 9**, and modern .NET engineering practices. Built as a technical portfolio and educational reference for enterprise-grade .NET development.

---

## Features

- Full Premier League 2024-25 season data (20 teams, 420 players, 380 matches)
- League standings table with real PL tiebreaker rules (GD, Goals For)
- Top scorers and top assists leaderboards
- Best attack / best defense rankings
- Paginated, filterable, sortable endpoints
- Realistic seeded data generated with Bogus using consistent random seeds
- Structured JSON logging with Serilog
- Global exception handling with ProblemDetails (RFC 7807)
- FluentValidation pipeline behavior via MediatR
- Health check endpoint
- Swagger / OpenAPI documentation
- Docker + Docker Compose for local SQL Server

---

## Architecture

This project implements **Clean Architecture** with strict layer isolation and unidirectional dependencies.

```
┌──────────────────────────────────────────┐
│              PremierLeague.Api           │  HTTP, Controllers, Middleware, DI
├──────────────────────────────────────────┤
│          PremierLeague.Application       │  CQRS, UseCases, DTOs, Validators
├──────────────────────────────────────────┤
│            PremierLeague.Domain          │  Entities, Enums, Exceptions, Interfaces
├──────────────────────────────────────────┤
│        PremierLeague.Infrastructure      │  EF Core, Repositories, Seeders, Logging
└──────────────────────────────────────────┘
```

### Dependency Rule
```
Api → Application → Domain ← Infrastructure
```
Infrastructure implements contracts defined in Domain. Application only knows about Domain. The Domain has zero external dependencies.

### Request Flow
```
HTTP Request
   → ExceptionHandlingMiddleware
   → Controller (thin — only dispatch)
   → IMediator.Send(Query)
   → LoggingBehavior (pipeline)
   → ValidationBehavior (pipeline — FluentValidation)
   → QueryHandler
       → IUnitOfWork → Repository → EF Core → SQL Server
   → Response DTO
   → HTTP Response (200/404/422/500)
```

---

## Project Structure

```
src/
├── PremierLeague.Domain/
│   ├── Entities/          # Team, Player, Match, Season, LeagueStanding, PlayerStatistic
│   ├── Enums/             # PlayerPosition, MatchStatus
│   ├── Exceptions/        # DomainException, NotFoundException
│   └── Interfaces/        # IRepository<T>, ITeamRepository, IUnitOfWork, ...
│
├── PremierLeague.Application/
│   ├── Common/
│   │   ├── Behaviors/     # LoggingBehavior, ValidationBehavior (MediatR pipeline)
│   │   ├── Exceptions/    # ValidationException
│   │   └── Models/        # Result<T>, PaginatedResult<T>
│   ├── Contracts/
│   │   └── Responses/     # TeamResponse, PlayerResponse, MatchResponse, StandingResponse, ...
│   └── Features/          # Organized by domain slice
│       ├── Teams/Queries/GetTeams | GetTeamById | GetBestDefense | GetBestAttack
│       ├── Players/Queries/GetPlayers | GetPlayerById | GetTopScorers | GetTopAssists
│       ├── Matches/Queries/GetMatches | GetMatchById | GetRecentMatches | GetUpcomingMatches
│       ├── Standings/Queries/GetStandings
│       └── Statistics/Queries/GetLeagueStatistics
│
├── PremierLeague.Infrastructure/
│   ├── Persistence/
│   │   ├── AppDbContext.cs
│   │   ├── Configurations/    # One IEntityTypeConfiguration<T> per entity
│   │   └── Repositories/      # GenericRepository<T> + domain-specific extensions
│   └── Seeders/               # DatabaseSeeder (orchestrator) + one seeder per entity
│
└── PremierLeague.Api/
    ├── Controllers/       # TeamsController, PlayersController, MatchesController, ...
    ├── Extensions/        # SwaggerExtensions
    ├── Middleware/        # ExceptionHandlingMiddleware
    └── Program.cs

tests/
├── PremierLeague.UnitTests/
│   ├── Domain/            # Entity invariant tests
│   └── Features/          # Handler tests with Moq
└── PremierLeague.IntegrationTests/
    └── Infrastructure/    # WebApplicationFactory + InMemory DB
```

---

## SOLID Principles Applied

| Principle | How |
|---|---|
| **S** ingle Responsibility | Each handler handles exactly one use case. Each seeder handles one entity type. |
| **O** pen/Closed | New features are new feature folders, not modifications to existing handlers. |
| **L** iskov Substitution | Repositories implement `IRepository<T>` and domain-specific interfaces interchangeably. |
| **I** nterface Segregation | `ITeamRepository` extends only `IRepository<Team>` with team-specific methods — not a god-interface. |
| **D** ependency Inversion | Handlers depend on `IUnitOfWork`, not `AppDbContext`. Infrastructure wires concrete types. |

---

## Architectural Decisions

### Why CQRS with MediatR?
Each query is an isolated, testable unit. Pipeline behaviors (logging, validation) apply uniformly without touching business logic. Adding a new use case means adding a new folder — zero modification to existing code.

### Why the Result<T> Pattern?
`Result<T>` makes success/failure explicit at the type level without throwing exceptions for expected domain errors (NotFound, ValidationFailure). Exceptions are reserved for truly unexpected failures.

### Why not Repository Pattern everywhere?
`GenericRepository<T>` handles boilerplate CRUD. Domain-specific repositories (`ITeamRepository`) add query methods that make sense for that aggregate. We avoid over-engineering simple queries while still enabling testability via interfaces.

### Why Mapster instead of AutoMapper?
Mapster is faster, has no known CVEs (AutoMapper 13.x had a high-severity vulnerability in May 2024), and works well for our read-heavy use case. Explicit projection in LINQ is preferred over convention-based mapping where it's clearer.

### Seeder Design
The `DatabaseSeeder` is idempotent — it checks for existing data before seeding. Each sub-seeder receives only what it needs. Statistics are derived from match results to ensure internal consistency (team's goals for = sum of goals seeded across matches).

---

## Getting Started

### Prerequisites
- .NET 9 SDK
- SQL Server 2019+ (or Docker)

### Local Development (SQL Server)

## Clone and restore

You can clone the repository using either HTTPS or SSH.

### Clone with HTTPS

```bash
git clone https://github.com/fpsoto/MyFirstDotNetPremierLeagueAPI.git
cd MyFirstDotNetPremierLeagueAPI
dotnet restore
```

### Clone with SSH

```bash
git clone git@github.com:fpsoto/MyFirstDotNetPremierLeagueAPI.git
cd MyFirstDotNetPremierLeagueAPI
dotnet restore
```

# Set your connection string in appsettings.Development.json
# (already configured for localhost with Windows Auth)

# Run — database is created and seeded automatically on first start
dotnet run --project src/PremierLeague.Api

# Swagger UI
open https://localhost:7207/swagger
```

### Docker Compose (SQL Server included)

```bash
docker-compose up --build
# API: http://localhost:8080/swagger
# SQL Server: localhost:1433 (sa / PremierLeague_2024!)
```

### EF Core Migrations

```bash
# Add a migration (run from solution root)
dotnet ef migrations add <MigrationName> \
  --project src/PremierLeague.Infrastructure \
  --startup-project src/PremierLeague.Api

# Apply migrations
dotnet ef database update \
  --project src/PremierLeague.Infrastructure \
  --startup-project src/PremierLeague.Api
```

---

## Running Tests

```bash
# All tests
dotnet test

# Unit tests only
dotnet test tests/PremierLeague.UnitTests

# Integration tests only
dotnet test tests/PremierLeague.IntegrationTests

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## API Endpoints

### Teams
| Method | Route | Description |
|---|---|---|
| GET | `/api/teams` | List teams (search, city filter, sort, pagination) |
| GET | `/api/teams/{id}` | Team detail with squad and current standing |
| GET | `/api/teams/best-defense?seasonId=&take=5` | Fewest goals conceded |
| GET | `/api/teams/best-attack?seasonId=&take=5` | Most goals scored |

### Players
| Method | Route | Description |
|---|---|---|
| GET | `/api/players` | List players (team, position, nationality, search, pagination) |
| GET | `/api/players/{id}?seasonId=` | Player detail with season statistics |
| GET | `/api/players/top-scorers?seasonId=&take=10` | Goals leaderboard |
| GET | `/api/players/top-assists?seasonId=&take=10` | Assists leaderboard |

### Matches
| Method | Route | Description |
|---|---|---|
| GET | `/api/matches?seasonId=` | All matches (team filter, status filter, pagination) |
| GET | `/api/matches/{id}` | Single match detail |
| GET | `/api/matches/recent?seasonId=&take=10` | Last completed matches |
| GET | `/api/matches/upcoming?seasonId=&take=10` | Next scheduled matches |

### Standings
| Method | Route | Description |
|---|---|---|
| GET | `/api/standings?seasonId=` | Full league table ordered by position |

### Statistics
| Method | Route | Description |
|---|---|---|
| GET | `/api/statistics/league?seasonId=` | Season summary (totals, averages, leaders) |

### Utilities
| Method | Route | Description |
|---|---|---|
| GET | `/health` | Health check |
| GET | `/swagger` | Swagger UI |

---

## Seeder Details

The seeder generates a complete Premier League 2024-25 season on first startup:

| Entity | Count |
|---|---|
| Seasons | 1 (active) |
| Teams | 20 (real PL 2024-25 clubs) |
| Players | 420 (21 per squad: 2 GK, 6 DEF, 8 MID, 5 FWD) |
| Matches | 380 (full home + away round-robin) |
| Standings | 20 (calculated from match results) |
| Player Statistics | 420 (consistent with team goal totals) |

Score distribution mirrors real PL patterns (most common: 1-1, 1-0, 2-1). A fixed random seed (42) ensures reproducible data across restarts.

---

## Technology Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 9, ASP.NET Core |
| ORM | Entity Framework Core 9 (SQL Server) |
| CQRS | MediatR 12 |
| Validation | FluentValidation 11 |
| Logging | Serilog (Console + File sinks) |
| Fake Data | Bogus 35 |
| Documentation | Swashbuckle / Swagger |
| Testing | xUnit, FluentAssertions, Moq |
| Containers | Docker, Docker Compose |

---

## Possible Extensions

- **Authentication**: Add JWT bearer auth to protect write endpoints
- **Write side**: Commands for creating/updating teams, recording match results
- **SignalR**: Real-time live match score updates
- **Caching**: Redis for standings and top-scorers (rarely change)
- **gRPC**: Alternative transport for internal service communication
- **Testcontainers**: Replace InMemory DB in integration tests with real SQL Server

---

## License

MIT — see [LICENSE](LICENSE).
