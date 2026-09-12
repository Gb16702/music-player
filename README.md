# Music Player

A portfolio-grade music player with an original, Apple Music-inspired interface.

## Stack

- Web: TanStack Start, React, TypeScript, TanStack Router and Query, Zustand, Valibot
- API: ASP.NET Core on .NET 10, PostgreSQL 18, EF Core
- Tooling: Bun workspaces, OpenAPI-generated client

## Layout

```text
apps/web       TanStack Start application
apps/api       ASP.NET Core solution
packages       Shared frontend packages and generated API client
```

## Local commands

```bash
bun install
docker compose up -d
bun run dev:web
bun run dev:api
```

Copy `.env.example` to `.env` for Docker/PostgreSQL. API secrets go in .NET User Secrets — see `AGENTS.md`.
