# Exeal URL Shortener

A full-stack URL shortener with click tracking, statistics, and Auth0-based authentication.

The repository is a monorepo with two independently deployable services:

| Directory | Description |
|-----------|-------------|
| [`backend/`](backend/README.md) | ASP.NET Core 10 REST API (hexagonal architecture, PostgreSQL) |
| [`frontend/`](frontend/README.md) | React 19 SPA (Create React App, Tailwind CSS) |

## Architecture overview

```
Browser → React SPA → ASP.NET Core API → PostgreSQL
               ↑                ↑
           Auth0 JWT       Auth0 JWT validation
```

- Authentication is handled entirely by Auth0. The SPA obtains a JWT and passes it as a `Bearer` token to every API call.
- The backend validates JWTs and enforces `[Authorize]` on all management endpoints.
- URL resolution (`/{slug}`) is public and redirects the visitor while tracking the click.

## CI/CD

Both services have independent GitHub Actions workflows under `.github/workflows/`:

- `backend-deploy.yml` — builds, tests, packages to `ghcr.io/exeal-es/url-shortener`, and triggers a Coolify deploy.
- `frontend-deploy.yml` — installs, builds the React app (environment variables injected at build time), packages to `ghcr.io/exeal-es/urlshortener-web`, and triggers a Coolify deploy.

Pushes to `main` trigger the relevant workflow only when files in that service's directory change.

## License

MIT © [Exeal](https://www.exeal.com)
