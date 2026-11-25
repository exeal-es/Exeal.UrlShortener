# Exeal URL Shortener

A full-stack URL shortener with click tracking, statistics, and Zitadel-based authentication.

The repository is a monorepo with two independently deployable services:

| Directory | Description |
|-----------|-------------|
| [`backend/`](backend/README.md) | ASP.NET Core 10 REST API (hexagonal architecture, PostgreSQL) |
| [`frontend/`](frontend/README.md) | React 19 SPA (Create React App, `@exeal/design`) |

## Architecture overview

```
Browser → React SPA → ASP.NET Core API → PostgreSQL
               ↑                ↑
          Zitadel JWT     Zitadel JWT validation
```

- Authentication is handled by Exeal's Zitadel (`auth.exeal.com`, managed in [`exeal-es/infra`](https://github.com/exeal-es/infra)): only Google accounts of Exeal's Workspace can sign in. The SPA obtains a JWT and passes it as a `Bearer` token to every API call.
- The backend validates JWTs and enforces `[Authorize]` on all management endpoints.
- URL resolution (`/{slug}`) is public and redirects the visitor while tracking the click.

## CI/CD

Both services have independent GitHub Actions workflows under `.github/workflows/`:

- `backend-deploy.yml` — builds, tests, packages to `ghcr.io/exeal-es/url-shortener`, and triggers a Coolify deploy.
- `frontend-deploy.yml` — installs, builds the React app (environment variables injected at build time) and uploads `build/` to Cloudflare Pages (project `urlshortener-web`): pushes to `main` go to production (admin.exeal.com), PRs get a preview URL.

Pushes to `main` trigger the relevant workflow only when files in that service's directory change.

## License

MIT © [Exeal](https://www.exeal.com)
