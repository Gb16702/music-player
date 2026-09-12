# Music Player

Portfolio music player, Apple Music (Windows) inspired UI. Platform owns accounts, profiles, likes, playlists. Spotify is optional.

## Stack

- Web: TanStack Start, React, TypeScript, TanStack Router/Query, Zustand, Valibot, Tailwind
- API: ASP.NET Core, .NET 10, Clean Architecture
- Data: PostgreSQL 18, EF Core, ASP.NET Identity (`Guid`)
- Tooling: Bun, Biome, OpenAPI client (`@hey-api/openapi-ts`)

## Layout

```text
apps/api/              .NET solution
apps/web/              TanStack Start
packages/api-client/   Generated client — regenerate, do not edit
```

Area context: `apps/api/AGENTS.md`, `apps/web/AGENTS.md`, `packages/api-client/AGENTS.md`.

## Current work: registration

```text
RegisterUserCommand → RegisterUserHandler
  → IIdentityService → IUserProfileRepository → IUnitOfWork
```

Done: handler + DI (`AddApplication`), persistence, 9 backend tests, frontend status page.

Next:
1. `POST /api/v1/auth/register` + tests

Blockers: no HTTP endpoint yet.

## Commands

```powershell
bun install && dotnet restore apps/api/MusicPlayer.slnx
docker compose up -d
bun run dev:web    # :3000
bun run dev:api    # :5100
bun run generate:api-client
dotnet test apps/api/MusicPlayer.slnx
bun run test:web && bun run build:web
```

Secrets: `.env` for Docker; API User Secrets for `ConnectionStrings:Database`, `Spotify:ClientId`, `Spotify:ClientSecret`. Never commit secrets.
