using Dapper;
using Exeal.UrlShortener.Ports.Output;

namespace Exeal.UrlShortener.Infra;

public class PostgresClickTracker : IClickTracker
{
    private readonly string _connectionString;

    public PostgresClickTracker(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task RegisterAsync(string slug, string ipAddress, string userAgent)
    {
        using var connection = new Npgsql.NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(
            @"INSERT INTO clicks (slug, ip_address, user_agent, created_at) 
              VALUES (@Slug, @IpAddress, @UserAgent, @CreatedAt)",
            new
            {
                Slug = slug,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = DateTimeOffset.UtcNow
            });
    }

    public async Task<int> GetClickCountAsync(string slug)
    {
        using var connection = new Npgsql.NpgsqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM clicks WHERE slug = @Slug",
            new { Slug = slug });
    }

    public async Task<int> GetUniqueVisitorCountAsync(string slug)
    {
        using var connection = new Npgsql.NpgsqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(DISTINCT ip_address) FROM clicks WHERE slug = @Slug",
            new { Slug = slug });
    }
} 