# Exeal URL Shortener — Web

React 19 single-page application for managing short URLs. Authenticates with Zitadel and communicates with the [backend API](../backend/README.md).

## Tech stack

- **React 19** with React Router 7
- **OIDC** (`react-oidc-context`, `oidc-client-ts`) against Zitadel for authentication
- **[`@exeal/design`](https://github.com/exeal-es/exeal-design)** for styling (private repo: installing needs read access to it). `src/index.css` only holds the app's own layout
- **qrcode.react** for QR code generation

## Pages and components

| Route | Component | Description |
|-------|-----------|-------------|
| `/` | `Profile` / `UrlList` / `UrlShortenerForm` | Dashboard: list links, create new ones |
| `/links/:slug/details` | `LinkDetails` | Click statistics and QR code for a link |
| `/links/:slug/edit` | `LinkEdit` | Edit destination URL and title |
| `/callback` | `AuthCallback` | Return from Zitadel; goes back to the page that asked to log in |

## Getting started

### Prerequisites

- Node.js 22
- A running backend API (see [`../backend`](../backend/README.md))
- An `@exeal.com` Google account

### Environment variables

Copy `.env.dist` to `.env.local`; its values already point at the local Zitadel client (`http://localhost:3000`):

```bash
cp .env.dist .env.local
```

| Variable | Description |
|----------|-------------|
| `REACT_APP_OIDC_AUTHORITY` | Zitadel issuer (`https://auth.exeal.com`) |
| `REACT_APP_OIDC_CLIENT_ID` | Client ID of the SPA, also the audience the API expects |
| `REACT_APP_OIDC_ORG_ID` | ID of the Zitadel organization that can sign in (Exeal Staff) |
| `REACT_APP_API_BASE_URL` | Base URL of the backend API (e.g. `http://localhost:5030`) |

### Run in development

```bash
npm install
npm start
```

The app will be available at http://localhost:3000.

### Run tests

```bash
npm test
```

### Build for production

```bash
npm run build
```

The production-ready static files are output to `build/`.

## Deployment

CI (`.github/workflows/frontend-deploy.yml`) builds the app and uploads `build/` to Cloudflare Pages (project `urlshortener-web`). Pages serves `index.html` for any unknown route, so client-side routes work on reload.

> Note: environment variables are baked in at build time by Create React App. CI passes them from the repo's Actions variables during `npm run build`. PR previews cannot log in: their URLs are not registered in Zitadel.

## License

MIT © [Exeal](https://www.exeal.com)
