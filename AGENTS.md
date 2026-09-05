# AGENTS.md

Guidance for OpenCode working in **EIMS** (Education Institute Management System) — ASP.NET Core 8.0 MVC, EF Core 7.0. See `CLAUDE.md` for the full write-up; this file is the compact operating guide.

## Architecture (strict)

6-layer dependency chain, **dependencies flow downward only** — controllers must never call repositories or use the DbContext directly:

```
SMS_App → SMS.BLL (Managers) → SMS.DAL (Repositories) → SMS.DB (DbContext) → SMS.Entities
SMS.Frameworks = shared utilities (no upward deps)
```

- Interfaces in `SMS.*/Contracts/`, implementations in `SMS.*/Managers|Repositories/`.
- All managers/repositories registered by hand in `SMS_App/Configurations/DependencyInjectionConfiguration.cs` (`Addservices()`) — a new CRUD feature must add its `I...Repository/Repository` and `I...Manager/Manager` there.

## Build, run, verify

```bash
dotnet build                                     # root: builds the full solution
dotnet run --project SMS_App/SMS_App.csproj      # https://localhost:5001
dotnet ef database update                        # run from SMS.DB/ (uses its DesignTimeDbContextFactory)
```

- **There is NO test project.** Verify changes by `dotnet build` + running the app.
- AutoMapper profile: `SMS_App/Utilities/AutoMapperConfiguration/AutoMapperProfile.cs` (injected as `IMapper`).
- Hangfire is **disabled by default** (`Hangfire.IsEnabled=false`); dashboard only exists when enabled.

## Database: the biggest gotchas

- `SMS_App/appsettings.json` → `DatabaseProvider` selects `"SqlServer"` or `"PostgreSQL"`. Only `DefaultConnection` is read, and it is **provider-dependent**:
  - **SqlServer**: value is AES-encrypted; decrypted at runtime with env vars `AES_KEY`/`AES_IV` (defaults `"1234567890123456"`). Producing a new one requires the `Resources/deployment/EncryptConnString/` console app.
  - **PostgreSQL**: value is used as **plain text**.
  - Multiple per-host profiles (`Mac_Docker_DefaultConnection`, `Noble_DefaultConnection`, …) exist; to switch hosts, copy the profile's value into `DefaultConnection` — the app otherwise ignores them.
- On startup `Program.cs` runs `CanConnectAsync()` → `EnsureCreatedAsync()` → `DbSeeder.SeedAsync()`. Schema is created from the EF model; **do not assume migrations are applied at runtime**.
- **SqlServer** schema changes use code-first migrations (run from `SMS.DB/`, add with `-o Migrations_SqlServer`). **PostgreSQL is NOT migration-driven** — it is provisioned from a dump (`Resources/docker/setup-eims-postgres.ps1` creates a `postgres:16` container on port 5433 and restores `eimsdb_noble.dump`).

## Auth & areas

- Claim-based authorization, **not role-based**: policies live in `SMS_App/Configurations/AuthorizationPolicies.cs`, action policies named e.g. `[Authorize(Policy = "CreateStudentPolicy")]`. A global `AuthorizeFilter` in `Program.cs` requires an authenticated user on every route.
- Areas: `API`, `Identity`, `SMSAPP`, `Student` — routed `{area:exists}/{controller=Home}/{action=Index}/{id?}`.

## Conventions that differ from defaults

- All I/O `async`/`await`, methods suffixed `Async`.
- `[ValidateAntiForgeryToken]` on every POST/PUT/DELETE action.
- `QueryTrackingBehavior.NoTracking` is the default; use `.AsTracking()` only when updating.
- Avoid N+1: pre-load related data in the controller via `Dictionary<K,V>` caches or `.Include()` — never query the DB inside a view loop (see `PERFORMANCE_FIXES.md`).
- ViewModels named `[Entity][Operation]VM` (e.g. `StudentCreateVM`).
- Format `<input type="date">` values with explicit `yyyy-MM-dd` (culture-default `ToString()` breaks on Linux deploys). `Npgsql.EnableLegacyTimestampBehavior` is enabled in `Program.cs`; mind `DateTime.Kind` with PostgreSQL.

## Misc

- `global.json` pins SDK `8.0.415`; product version built from `Directory.Build.props` (`VersionPrefix`, bumped by hand; build number supplied at publish).
- Use the root `SchoolManagementSystem.sln`, not the stray `SMS_App/SMS_App.sln`.
- Data protection keys persist to `SMS_App/Keys/` (don't delete; shared-hosting stability).
- Raw SQL (stored procs, views, data migration) lives in `Resources/sql*` and `Resources/StudentMigrationScripts/` — run outside EF.

## References

- `CLAUDE.md` — full architecture/CRUD feature checklist and settings reference
- `CODING_STANDARDS.md` (+ per-layer copies) — naming, SOLID, layer responsibilities
- `PERFORMANCE_FIXES.md` — N+1 optimization example for `StudentFeeAllocations`
- `SMS_App/SiteMap.Config` — navigation menu structure