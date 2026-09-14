# Music Player

Portfolio music player, Apple Music (Windows) inspired UI. Platform owns accounts, profiles, likes, playlists.

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

## Auth (target)

Entry page: social buttons (Google, Spotify) + switch between magic link and email/password.

| Method | Notes |
|---|---|
| Google OAuth | Primary passwordless path |
| Spotify OAuth | Creates account + stores API tokens; no separate link step |
| Magic link | Email only; no password on our app |
| Email + password | Fallback |

Rules:
- App account (`ApplicationUser`) is always the source of truth.
- `UserProfile` (display name, avatar, etc.) is app-owned — never copy provider name/photo as canonical.
- Spotify email is not guaranteed (`user-read-email` scope + verified account required).
- Spotify signup stores tokens immediately; email signup links Spotify later via integration endpoint.

## Onboarding (target)

Required after first auth, regardless of method:
- Choose display name (username on platform)
- Choose profile image
- Optional skippable steps for a styled multi-step flow

`OnboardingCompleted` gates app access. Provider data may pre-fill suggestions only.

Current `POST /auth/register` (email + password + displayName) is interim — displayName will move to onboarding.

## Current API work

Auth: register + login (cookie), typed errors, atomic transaction, validation tests. DB integration tests require Docker (skipped otherwise).

Next: magic link, social OAuth, onboarding refactor.

Test in Scalar: `http://localhost:5100/scalar`

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

Secrets: `.env` for Docker; API User Secrets for `ConnectionStrings:Database`, `Spotify:ClientId`, `Spotify:ClientSecret`, `Google:ClientId`, `Google:ClientSecret`. Never commit secrets.
