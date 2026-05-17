# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**EIMS** (Education Institute Management System) is a comprehensive school/institute management system built with ASP.NET Core 8.0 MVC, managing student records, academics, employee data, financial transactions, and attendance tracking.

## Tech Stack

- **Framework**: ASP.NET Core 8.0 MVC with Razor Views
- **Language**: C# / .NET 8.0
- **ORM**: Entity Framework Core 7.0 (code-first migrations)
- **Database**: MS SQL Server
- **Auth**: ASP.NET Core Identity with claim-based authorization policies
- **Background Jobs**: Hangfire (disabled by default)
- **Logging**: Serilog → SQL Server sink
- **Mapping**: AutoMapper 15.1.1
- **External**: GreenWeb SMS API, SMTP email, RDLC ReportViewer

## Build & Run Commands

```powershell
# Build entire solution
dotnet build

# Run application (default: https://localhost:5001)
dotnet run --project SMS_App\SMS_App.csproj

# EF Core migrations (run from SMS_App directory)
dotnet ef database update
dotnet ef migrations add <MigrationName>
dotnet ef migrations remove
```

## Architecture — 6-Layer Dependency Chain

```
SMS_App          → Presentation (Controllers, Views, ViewModels)
SMS.BLL          → Business Logic (Manager classes)
SMS.DAL          → Data Access (Repository pattern)
SMS.DB           → EF Core DbContext + Migrations
SMS.Entities     → Domain models, enums, ViewModels (no logic)
SMS.Frameworks   → Shared utilities, tag helpers, encryption
```

**Strict rule**: Dependencies flow downward only. Controllers → Managers → Repositories. Never skip layers; controllers must never call repositories directly.

## Key Architectural Patterns

### Repository Pattern
- All repositories inherit `GenericRepository<T>` (`SMS.DAL/Repositories/Base/`)
- Interfaces live in `SMS.DAL/Contracts/`, implementations in `SMS.DAL/Repositories/`
- Standard async CRUD: `GetByIdAsync()`, `GetAllAsync()`, `AddAsync()`, `UpdateAsync()`, `DeleteAsync()`

### Manager Pattern (Business Logic)
- All business logic in Manager classes under `SMS.BLL/Managers/`
- Interfaces in `SMS.BLL/Contracts/`
- Managers inject repositories; controllers inject managers only

### Claim-Based Authorization (Not Role-Based)
- All authorization uses policies defined in `SMS_App/Configurations/AuthorizationPolicies.cs`
- Decorate actions with `[Authorize(Policy = "[EntityName]Policy")]`
- Claims stored in `ClaimStores` entity and assigned via roles
- Global auth filter in `Program.cs` requires authenticated user for all routes

### AutoMapper
- Profiles configured in `SMS_App/Utilities/AutoMapperConfiguration/AutoMapperProfile.cs`
- Injected as `IMapper` into managers and controllers

### Areas
Four areas: `API`, `Identity`, `SMSAPP`, `Student` — routed as `{area:exists}/{controller=Home}/{action=Index}/{id?}`

## Configuration & Secrets

### Encrypted Connection Strings
Connection strings in `appsettings.json` are AES-encrypted. Decrypted in `Program.cs` using `AesEncryptionHelper.Decrypt()` with keys from environment variables `AES_KEY` and `AES_IV` (defaults: `"1234567890123456"` if unset).

### Key appsettings.json Settings
- `Hangfire.IsEnabled` — set to `true` to enable background jobs; dashboard at `/hangfire`
- `PhoneSMSSetup` — GreenWeb SMS API token and endpoint
- `Serilog.MinimumLevel` — default `"Error"`

### Data Protection Keys
Persisted to `SMS_App/Keys/` for shared hosting stability. App name: `"SMS_App"`.

### User Secrets ID
`aspnet-SchoolManagementSystem-75AC8697-780A-4215-A7FE-E83F96B3C344`

## Adding a Complete CRUD Feature

1. Define entity in `SMS.Entities/` with data annotations
2. Add `DbSet<T>` to `SMS.DB/ApplicationDbContext.cs`
3. `dotnet ef migrations add Add[EntityName]`
4. Create `I[Entity]Repository` in `SMS.DAL/Contracts/` and implement in `SMS.DAL/Repositories/`
5. Create `I[Entity]Manager` in `SMS.BLL/Contracts/` and implement in `SMS.BLL/Managers/`
6. Create ViewModels in `SMS_App/ViewModels/[Feature]/`
7. Create Controller at `SMS_App/Controllers/[Entity]Controller.cs` — inject manager only
8. Create Views at `SMS_App/Views/[Entity]/`
9. Register manager + repository in service collection (`Program.cs`)
10. Add authorization policy in `AuthorizationPolicies.cs` and apply `[Authorize(Policy = "...")]` to actions

## Code Conventions

- All I/O operations use `async/await`; method names suffixed with `Async`
- `[ValidateAntiForgeryToken]` on all POST/PUT/DELETE actions
- `QueryTrackingBehavior.NoTracking` by default in `ApplicationDbContext`; use `.AsTracking()` only when updating
- Avoid N+1 queries: pre-load all related data in the controller via `Dictionary<K,V>` caches or `.Include()`, never query the DB inside a view loop
- ViewModel naming: `[Entity][Operation]VM` (e.g., `StudentCreateVM`, `StudentListVM`)

## Notable Files

| File | Purpose |
|------|---------|
| `SMS_App/Program.cs` | Application entry point, DI registration, auth/session config |
| `SMS.DB/ApplicationDbContext.cs` | EF Core context, inherits `IdentityDbContext` |
| `SMS_App/Configurations/AuthorizationPolicies.cs` | All claim-based authorization policies |
| `SMS_App/appsettings.json` | App configuration (encrypted connection strings) |
| `SMS_App/GlobalUI.cs` | Global UI constants |
| `SchoolManagementSystem.sln` | Solution file |

## Reference Documentation

- `README.md` — Feature overview and setup instructions  
- `CODING_STANDARDS.md` (root and per-layer) — Naming conventions, SOLID patterns  
- `PERFORMANCE_FIXES.md` — N+1 query optimization example for StudentFeeAllocations  
- `SiteMap.Config` — Navigation menu structure
