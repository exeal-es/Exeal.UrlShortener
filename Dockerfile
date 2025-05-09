FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Exeal.UrlShortener.Api/Exeal.UrlShortener.Api.csproj", "Exeal.UrlShortener.Api/"]
COPY ["Exeal.UrlShortener/Exeal.UrlShortener.csproj", "Exeal.UrlShortener/"]
COPY ["Exeal.UrlShortener.Infra/Exeal.UrlShortener.Infra.csproj", "Exeal.UrlShortener.Infra/"]
RUN dotnet restore "Exeal.UrlShortener.Api/Exeal.UrlShortener.Api.csproj"

# Copy the rest of the code
COPY . .

# Build and publish
RUN dotnet build "Exeal.UrlShortener.Api/Exeal.UrlShortener.Api.csproj" -c Release -o /app/build
RUN dotnet publish "Exeal.UrlShortener.Api/Exeal.UrlShortener.Api.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "Exeal.UrlShortener.Api.dll"] 