# Exeal URL Shortener — API

ASP.NET Core 10 REST API for creating and managing short URLs. Built with a hexagonal (ports & adapters) architecture backed by PostgreSQL.

## Architecture

```
Exeal.UrlShortener        — domain core (use cases, ports)
Exeal.UrlShortener.Infra  — adapters (PostgreSQL, Redis cache, slug generator)
Exeal.UrlShortener.Api    — HTTP entry point (controllers, Auth0 JWT validation)
```

## API endpoints

All management endpoints require a valid Auth0 JWT (`Authorization: Bearer <token>`).

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| `POST` | `/api/shorturl` | Required | Create a short URL |
| `GET` | `/api/shorturl` | Required | List short URLs (paginated) |
| `GET` | `/api/shorturl/{slug}/stats` | Required | Get click statistics for a link |
| `PATCH` | `/api/shorturl/{slug}` | Required | Update destination URL or title |
| `GET` | `/{slug}` | Public | Redirect to destination URL (tracks click) |

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/products/docker-desktop)

### Run PostgreSQL locally

```bash
docker run --name pg-urlshortener \
  -e POSTGRES_USER=devuser \
  -e POSTGRES_PASSWORD=devpass \
  -e POSTGRES_DB=urlshortener \
  -p 5432:5432 \
  -d postgres:16
```

Add the connection string to `Exeal.UrlShortener.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=urlshortener;Username=devuser;Password=devpass"
  }
}
```

To stop and remove the container:

```bash
docker stop pg-urlshortener && docker rm pg-urlshortener
```

### Run the API

```bash
dotnet run --project Exeal.UrlShortener.Api
```

The API will be available at http://localhost:5030.

### Run tests

```bash
dotnet test
```

## Docker

Build the image:

```bash
docker build . -t exeal/url-shortener-api
```

Run the container:

```bash
docker run --name url-shortener-api \
  -e "ConnectionStrings__DefaultConnection=Host=host.docker.internal;Port=5432;Database=urlshortener;Username=devuser;Password=devpass" \
  -p 5030:5030 \
  exeal/url-shortener-api
```

## License

MIT © [Exeal](https://www.exeal.com)
