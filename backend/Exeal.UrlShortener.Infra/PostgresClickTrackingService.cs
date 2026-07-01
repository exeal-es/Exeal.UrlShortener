using Dapper;
using Exeal.UrlShortener.Ports.Output;
using System.Security.Cryptography;
using System.Text;

namespace Exeal.UrlShortener.Infra;

public class PostgresClickTrackingService(string connectionString) : IClickTracker, IClickStatisticsProvider
{
    public async Task RegisterAsync(string slug, string ipAddress, string userAgent)
    {
        var visitorFingerprint = ComputeVisitorFingerprint(ipAddress, userAgent);

        using var connection = new Npgsql.NpgsqlConnection(connectionString);
        await connection.ExecuteAsync(
            @"INSERT INTO ""Clicks"" (""Slug"", ""VisitorFingerprint"", ""CreatedAt"") 
              VALUES (@Slug, @VisitorFingerprint, @CreatedAt)",
            new
            {
                Slug = slug,
                VisitorFingerprint = visitorFingerprint,
                CreatedAt = DateTimeOffset.UtcNow
            });
    }

    private string ComputeVisitorFingerprint(string ipAddress, string userAgent)
    {
        using var sha256 = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes(ipAddress + userAgent);
        var hashBytes = sha256.ComputeHash(inputBytes);
        return Convert.ToBase64String(hashBytes);
    }

    public async Task<int> GetClickCountAsync(string slug)
    {
        using var connection = new Npgsql.NpgsqlConnection(connectionString);
        return await connection.ExecuteScalarAsync<int>(
            @"SELECT COUNT(*) FROM ""Clicks"" WHERE ""Slug"" = @Slug",
            new { Slug = slug });
    }

    public async Task<int> GetUniqueVisitorCountAsync(string slug)
    {
        using var connection = new Npgsql.NpgsqlConnection(connectionString);
        return await connection.ExecuteScalarAsync<int>(
            @"SELECT COUNT(DISTINCT ""VisitorFingerprint"") FROM ""Clicks"" WHERE ""Slug"" = @Slug",
            new { Slug = slug });
    }
}