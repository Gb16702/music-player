# API

Clean Architecture: Domain → Application ← Infrastructure; API composes both.

## Projects

- `Domain` — entities, invariants
- `Application` — use cases, `Abstractions/` ports, `DependencyInjection.cs`
- `Infrastructure` — EF Core, Identity, Spotify options
- `Api` — endpoints, `Program.cs`

## Auth

Interim: `POST /api/v1/auth/register`, `POST /api/v1/auth/login` (cookie auth).

Target: Google + Spotify OAuth (login/callback), magic link, onboarding decoupled from register. Spotify dev redirect: `http://127.0.0.1:5100/signin-spotify`.

`ApplicationUser` = auth account. `UserProfile` = app-owned public identity. Provider data never canonical.

`IUnitOfWork` = scoped `MusicPlayerDbContext`. Identity + `UserProfile` 1:1, cascade delete.

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

Real DB integration tests for registration (success, duplicate email, invalid password).
