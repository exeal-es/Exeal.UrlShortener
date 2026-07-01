# Exeal.UrlShortener.Api

A simple, clean and extensible URL shortener API built with ASP.NET Core and following a hexagonal architecture.  
This API allows creating short URLs, resolving them with tracking, and retrieving usage statistics.

---

## 🚀 Getting Started

### 1. Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/products/docker-desktop)

### 2. Run PostgreSQL locally (dev environment)

Use this command to spin up a PostgreSQL instance using Docker:

```bash
docker run --name pg-urlshortener \
  -e POSTGRES_USER=devuser \
  -e POSTGRES_PASSWORD=devpass \
  -e POSTGRES_DB=urlshortener \
  -p 5432:5432 \
  -d postgres:16
```

Connection string for your `appsettings.Development.json`:

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

### 3. Run the API

```bash
dotnet run --project Exeal.UrlShortener.Api
```

The API will be available at:
http://localhost:5030

### 4. Running with Docker

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

---

## 📝 License

MIT © [Exeal](https://www.exeal.com)
