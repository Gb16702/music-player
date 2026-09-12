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

## State

Status page at `/` works. App shell, auth, player not built yet. Wait for stable registration API before auth forms.
