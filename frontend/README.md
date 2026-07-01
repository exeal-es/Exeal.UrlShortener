# Exeal URL Shortener — Web

React 19 single-page application for managing short URLs. Authenticates with Auth0 and communicates with the [backend API](../backend/README.md).

## Tech stack

- **React 19** with React Router 7
- **Auth0** (`@auth0/auth0-react`) for authentication
- **Tailwind CSS** for styling
- **qrcode.react** for QR code generation

## Pages and components

| Route | Component | Description |
|-------|-----------|-------------|
| `/` | `Profile` / `UrlList` / `UrlShortenerForm` | Dashboard: list links, create new ones |
| `/links/:slug/details` | `LinkDetails` | Click statistics and QR code for a link |
| `/links/:slug/edit` | `LinkEdit` | Edit destination URL and title |

## Getting started

### Prerequisites

- Node.js 22
- A running backend API (see [`../backend`](../backend/README.md))
- An Auth0 application configured for SPA

### Environment variables

Copy `.env.dist` to `.env.local` and fill in the values:

```bash
cp .env.dist .env.local
```

| Variable | Description |
|----------|-------------|
| `REACT_APP_AUTH0_DOMAIN` | Your Auth0 tenant domain (e.g. `example.eu.auth0.com`) |
| `REACT_APP_AUTH0_CLIENT_ID` | Auth0 SPA application client ID |
| `REACT_APP_AUTH0_AUDIENCE` | Auth0 API audience identifier |
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

## Docker

The `Dockerfile` serves the production build with nginx:

```bash
# Build the React app first
npm run build

# Build and run the image
docker build -t exeal/urlshortener-web .
docker run -p 80:80 exeal/urlshortener-web
```

> Note: environment variables are baked in at build time by Create React App. Pass them as `--build-arg` / environment variables during `npm run build`, not at container runtime.

## License

MIT © [Exeal](https://www.exeal.com)
