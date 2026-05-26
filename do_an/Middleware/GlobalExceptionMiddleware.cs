namespace do_an.Middleware;

/// <summary>
/// Middleware xu ly exception toan cuc
/// - Ghi log chi tiet loi
/// - Tra trang loi thân thien cho user
/// - Bao gom correlation ID de debug
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var correlationId = Guid.NewGuid().ToString("N")[..8];
            var path = context.Request.Path;
            var method = context.Request.Method;
            var user = context.User.Identity?.Name ?? "anonymous";

            _logger.LogError(ex,
                "Unhandled exception | CorrelationId: {CorrelationId} | User: {User} | {Method} {Path}",
                correlationId, user, method, path);

            await HandleExceptionAsync(context, ex, correlationId);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex, string correlationId)
    {
        context.Response.StatusCode = 500;

        // API request -> JSON response
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.ContentType = "application/json";
            var json = $$"""
            {
                "error": "Da xay ra loi he thong",
                "correlationId": "{{correlationId}}",
                "message": "Vui long lien he ho tro voi ma tren."
            }
            """;
            await context.Response.WriteAsync(json);
            return;
        }

        // HTML request -> redirect to error page
        context.Response.Redirect($"/Home/Error?correlationId={correlationId}");
    }
}
