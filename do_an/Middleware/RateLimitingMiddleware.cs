using System.Collections.Concurrent;

namespace do_an.Middleware;

/// <summary>
/// Middleware gioi han so request theo IP
/// - Login/Register: 10 request/phut
/// - Mac dinh: 30 request/phut
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;

    // Track: IP -> list of request timestamps
    private static readonly ConcurrentDictionary<string, List<DateTime>> _requestLog = new();

    private const int LoginLimit = 10;   // request/phut
    private const int GeneralLimit = 30;  // request/phut
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var path = context.Request.Path.Value?.ToLower() ?? "";

        // Bo qua rate limit cho localhost / loopback (phat trien)
        if (context.Connection.RemoteIpAddress != null &&
            System.Net.IPAddress.IsLoopback(context.Connection.RemoteIpAddress))
        {
            await _next(context);
            return;
        }

        // Xac dinh limit dua tren endpoint
        int limit = IsAuthEndpoint(path) ? LoginLimit : GeneralLimit;

        // Lay va cap nhat request log
        var now = DateTime.UtcNow;
        var timestamps = _requestLog.GetOrAdd(ip, _ => new List<DateTime>());

        bool rateLimitExceeded;
        lock (timestamps)
        {
            // Xoa cac request ngoai cua so
            timestamps.RemoveAll(t => now - t > Window);
            rateLimitExceeded = timestamps.Count >= limit;
            if (!rateLimitExceeded)
                timestamps.Add(now);
        }

        if (rateLimitExceeded)
        {
            _logger.LogWarning("Rate limit exceeded for IP {IP} on {Path}", ip, path);
            context.Response.StatusCode = 429;
            context.Response.Headers["Retry-After"] = "60";
            context.Response.ContentType = "application/json; charset=utf-8";
            var json = """{"error": "Qua nhieu request. Vui long thu lai sau."}""";
            await context.Response.WriteAsync(json);
            return;
        }

        // Cleanup định kỳ (mỗi 1000 IP)
        if (_requestLog.Count > 1000)
        {
            CleanupOldEntries();
        }

        await _next(context);
    }

    private static bool IsAuthEndpoint(string path)
    {
        return path.Contains("/account/login") ||
               path.Contains("/account/register") ||
               path.Contains("/api/aichat");
    }

    private static void CleanupOldEntries()
    {
        var now = DateTime.UtcNow;
        foreach (var kvp in _requestLog)
        {
            lock (kvp.Value)
            {
                kvp.Value.RemoveAll(t => now - t > Window);
                if (kvp.Value.Count == 0)
                    _requestLog.TryRemove(kvp.Key, out _);
            }
        }
    }
}
