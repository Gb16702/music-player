# API

Clean Architecture: Domain → Application ← Infrastructure; API composes both.

## Projects

- `Domain` — entities, invariants
- `Application` — use cases, `Abstractions/` ports, `DependencyInjection.cs`
- `Infrastructure` — EF Core, Identity, Spotify options
- `Api` — endpoints, `Program.cs`

## Registration

`RegisterUserHandler` registered via `AddApplication()`. No HTTP endpoint yet.

`IUnitOfWork` = scoped `MusicPlayerDbContext`.

Identity: `ApplicationUser` + `UserProfile` (1:1, cascade delete). No roles.

## Key paths

```text
Api/Program.cs, Endpoints/
Application/Users/Register/
Application/Abstractions/
Infrastructure/Identity/, Persistence/, DependencyInjection.cs
```

## EF migrations

From `apps/api`:

```powershell
dotnet ef migrations add Name -p src/MusicPlayer.Infrastructure -s src/MusicPlayer.Api --output-dir Persistence/Migrations
dotnet ef database update -p src/MusicPlayer.Infrastructure -s src/MusicPlayer.Api
```

## Tests

```powershell
dotnet test apps/api/MusicPlayer.slnx
```

Integration tests use dummy DB strings; no real DB tests yet.

## Next

Typed registration errors → atomic create → `POST /api/v1/auth/register` with Problem Details (`400`/`409`).
