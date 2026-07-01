using System.Collections.Concurrent;
using Exeal.UrlShortener.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Exeal.UrlShortener.Infra;

public record ClickEvent(string Slug, string IpAddress, string UserAgent);

public class BufferedClickTracker : IClickTracker, IDisposable
{
    private readonly ConcurrentQueue<ClickEvent> _buffer = new();
    private readonly Timer _timer;
    private readonly IClickTracker _inner;
    private readonly ILogger<BufferedClickTracker> _logger;

    public BufferedClickTracker(ILogger<BufferedClickTracker> logger, IClickTracker inner)
    {
        _logger = logger;
        _inner = inner;
        _timer = new Timer(FlushBuffer, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
    }

    public Task RegisterAsync(string slug, string ipAddress, string userAgent)
    {
        _buffer.Enqueue(new ClickEvent(slug, ipAddress, userAgent));
        return Task.CompletedTask;
    }

    private async void FlushBuffer(object? state)
    {
        if (_buffer.IsEmpty) return;

        var batch = new List<ClickEvent>();
        while (_buffer.TryDequeue(out var click))
        {
            batch.Add(click);
        }

        if (batch.Count > 0)
        {
            try
            {
                _logger.LogInformation("Flushing {Count} clicks to DB", batch.Count);
                foreach (var clickEvent in batch)
                {
                    await _inner.RegisterAsync(clickEvent.Slug, clickEvent.IpAddress, clickEvent.UserAgent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error flushing click buffer to DB");
                // O puedes re-enqueuearlos si quieres resiliencia extrema
            }
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
        FlushBuffer(null); // Último intento al apagar
    }
}