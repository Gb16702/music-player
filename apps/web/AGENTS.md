# Web

TanStack Start app. API via `@music-player/api-client`.

## Stack

TanStack Router, Query (SSR), Valibot, Zustand (client state only), Tailwind.

## Patterns

- Query options in `src/queries/` with Valibot validation on API responses
- Server state in Query; player/queue UI state in Zustand
- SSR for shell and initial route data

## Key paths

```text
src/router.tsx, config/env.ts, queries/, routes/
```

## Commands

```powershell
bun run test:web
bun run build:web
```

## Auth UI (target)

Entry page: Google + Spotify buttons, switch magic link / email+password.

After any auth method → onboarding (display name, avatar, optional skippable steps) before app access.

## State

Status page at `/` works. Auth and onboarding not built yet.
