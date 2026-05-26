using Microsoft.Extensions.Diagnostics.HealthChecks;
using do_an.Data;

namespace do_an.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly AppDbContext _context;

    public DatabaseHealthCheck(AppDbContext context) => _context = context;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? HealthCheckResult.Healthy("Ket noi database OK")
                : HealthCheckResult.Unhealthy("Khong the ket noi database");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Loi ket noi database", ex);
        }
    }
}
