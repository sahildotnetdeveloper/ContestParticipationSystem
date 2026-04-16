# Contest Participation System

Production-ready backend API for managing contests, role-based participation, submissions, scoring, leaderboards, and user history using ASP.NET Core Web API, EF Core, PostgreSQL, JWT authentication, and rate limiting.

## Tech Stack

- ASP.NET Core Web API (`net8.0`)
- Entity Framework Core
- PostgreSQL via `Npgsql`
- JWT Bearer Authentication
- Built-in ASP.NET Core rate limiting middleware
- Swagger / OpenAPI

## Architecture

The solution follows a clean architecture layout:

- `src/ContestParticipationSystem.Api`
  - Controllers
  - Middleware
  - JWT, Swagger, rate limiting, and application bootstrap
- `src/ContestParticipationSystem.Application`
  - DTOs
  - Business services
  - Interfaces
  - Common response contracts
  - Domain exceptions
- `src/ContestParticipationSystem.Domain`
  - Entities
  - Enums
- `src/ContestParticipationSystem.Infrastructure`
  - EF Core `DbContext`
  - Repository implementations
  - JWT token generation
  - Seed data

## Project Structure

```text
src/
  ContestParticipationSystem.Api/
  ContestParticipationSystem.Application/
  ContestParticipationSystem.Domain/
  ContestParticipationSystem.Infrastructure/
database/
  001_initial_schema.sql
postman/
  ContestParticipationSystem.postman_collection.json
```

## Setup

1. Update the PostgreSQL connection string in [appsettings.json](/C:/Users/admijn/Documents/New%20project/src/ContestParticipationSystem.Api/appsettings.json).
2. Ensure the JWT secret is at least 32 characters.
3. Restore and build:

```powershell
$env:DOTNET_CLI_HOME="C:\Users\admijn\Documents\New project\.dotnet"
$env:LOCALAPPDATA="C:\Users\admijn\Documents\New project\.localappdata"
$env:APPDATA="C:\Users\admijn\Documents\New project\.appdata"
$env:TEMP="C:\Users\admijn\Documents\New project\.temp"
$env:TMP="C:\Users\admijn\Documents\New project\.temp"
$env:NUGET_PACKAGES="C:\Users\admijn\Documents\New project\.nuget\packages"
dotnet restore ContestParticipationSystem.sln
dotnet build ContestParticipationSystem.sln
dotnet run --project src\ContestParticipationSystem.Api
```

4. The app will create the database schema on startup and seed these users:

- `admin@contest.local` / `Admin@123`
- `vip@contest.local` / `VipUser@123`
- `user@contest.local` / `NormalUser@123`

## Key Rules Implemented

- `Admin` can manage contests, questions, and user roles.
- `VIP` contests can only be participated in by `Admin` and `VipUser`.
- `Normal` contests can be participated in by `Admin`, `VipUser`, and `NormalUser`.
- Guests can only read contest and leaderboard endpoints.
- Users cannot start a contest before its start time.
- Users cannot submit after the end time.
- Duplicate submissions are blocked.
- Empty submissions are rejected.
- Invalid question IDs and option IDs are rejected.
- Single-select and true/false questions must contain exactly one selected option.
- Multi-select requires an exact full match for scoring. Partial matches score `0`.
- All submission flows are async and wrapped in serializable transactions to reduce concurrency issues.
- Rate limiting is applied at `10 requests/second` per authenticated user or guest IP.

## Database

- EF Core models live under [AppDbContext.cs](/C:/Users/admijn/Documents/New%20project/src/ContestParticipationSystem.Infrastructure/Persistence/AppDbContext.cs).
- SQL bootstrap script lives at [001_initial_schema.sql](/C:/Users/admijn/Documents/New%20project/database/001_initial_schema.sql).
- Indexes are added for:
  - `Users.Email`
  - `Participations.UserId`
  - `Participations.ContestId`
  - `Participations(UserId, ContestId)` unique
  - contest time windows
  - question and option ordering constraints

## API Endpoints

### Auth

- `POST /api/auth/register`
- `POST /api/auth/login`

### Contests

- `GET /api/contests`
- `GET /api/contests/{contestId}`
- `POST /api/contests` `Admin`
- `PUT /api/contests/{contestId}` `Admin`
- `DELETE /api/contests/{contestId}` `Admin`

### Questions

- `GET /api/contests/{contestId}/questions` `Admin`
- `POST /api/contests/{contestId}/questions` `Admin`
- `PUT /api/questions/{questionId}` `Admin`
- `DELETE /api/questions/{questionId}` `Admin`

### Participation

- `POST /api/contests/{contestId}/participation/start`
- `POST /api/contests/{contestId}/participation/submit`

### Leaderboard

- `GET /api/contests/{contestId}/leaderboard`

### Users

- `GET /api/users/me/history`
- `PATCH /api/users/{userId}/role` `Admin`

## Response Shape

All endpoints return the standard envelope:

```json
{
  "success": true,
  "message": "Contest created successfully.",
  "data": {}
}
```

## Example Request

```http
POST /api/contests/{contestId}/participation/submit
Authorization: Bearer <token>
Content-Type: application/json

{
  "answers": [
    {
      "questionId": "8f3b5540-bfd4-4325-a736-14f44049ee35",
      "selectedOptionIds": [
        "0f43b0df-1b83-4fda-ae06-eb2b1a93bdcb"
      ]
    }
  ]
}
```

## Postman

Import [ContestParticipationSystem.postman_collection.json](/C:/Users/admijn/Documents/New%20project/postman/ContestParticipationSystem.postman_collection.json) into Postman, then set:

- `baseUrl`
- `token`
- `contestId`
- `questionId`
- `userId`

## Notes

- This implementation targets PostgreSQL. The repository and EF configuration can be adapted to SQL Server if needed.
- The application uses automatic database creation on startup to keep onboarding simple in a fresh environment.
- If you prefer EF migration classes instead of the included SQL script, run `dotnet ef migrations add InitialCreate --project src\ContestParticipationSystem.Infrastructure --startup-project src\ContestParticipationSystem.Api` after package restore succeeds.
