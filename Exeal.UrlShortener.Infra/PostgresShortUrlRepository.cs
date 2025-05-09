using Dapper;
using Exeal.UrlShortener.Ports.Output;
using Npgsql;

namespace Exeal.UrlShortener.Infra;

public class PostgresShortUrlRepository : IShortUrlRepository
{
    private readonly string _connectionString;

    public PostgresShortUrlRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<bool> ExistsAsync(string slug)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var exists = await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM \"ShortUrls\" WHERE \"Slug\" = @Slug)",
            new { Slug = slug });
        return exists;
    }

    public async Task SaveAsync(ShortUrl shortUrl)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var sql = @"
            INSERT INTO ""ShortUrls"" (""Slug"", ""DestinationUrl"", ""CreatedAt"")
            VALUES (@Slug, @DestinationUrl, @CreatedAt)";

        try
        {
            await connection.ExecuteAsync(sql, new
            {
                shortUrl.Slug,
                shortUrl.DestinationUrl,
                shortUrl.CreatedAt
            });
        }
        catch (PostgresException ex) when (ex.SqlState == "23505") // Unique violation
        {
            throw new InvalidOperationException("Slug already exists.", ex);
        }
    }

    public async Task<ShortUrl?> LoadBySlugAsync(string slug)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var sql = @"
            SELECT ""Slug"", ""DestinationUrl"", ""CreatedAt""
            FROM ""ShortUrls""
            WHERE ""Slug"" = @Slug";

        return await connection.QuerySingleOrDefaultAsync<ShortUrl>(sql, new { Slug = slug });
    }
}