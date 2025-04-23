namespace Api;

public static class HealthCheckExtensions
{
    /// <summary>
    /// Adds the health check endpoints.
    /// </summary>
    /// <param name="app"></param>
    public static void MapHealthChecks(this WebApplication app)
    {
        // this endpoint returns HTTP 200 if all "liveness" checks have passed, otherwise, it returns HTTP 500
        app.MapHealthChecks("/liveness", new HealthCheckOptions()
        {
            Predicate = registration => registration.Tags.Contains(HealthCheckType.Liveness)
        });

        // this endpoint returns HTTP 200 if all "readiness" checks have passed, otherwise, it returns HTTP 500
        app.MapHealthChecks("/ready", new HealthCheckOptions()
        {
            Predicate = registration => registration.Tags.Contains(HealthCheckType.Readiness)
        });
    }

    /// <summary>
    /// Adds the liveness and readiness health checks for the application.
    /// </summary>
    /// <param name="builder"></param>
    public static void AddHealthChecks(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("liveness", () => HealthCheckResult.Healthy(), tags: [HealthCheckType.Liveness])
            .AddCheck("ready", () => HealthCheckResult.Healthy(), tags: [HealthCheckType.Readiness]);
    }
}

public static class HealthCheckType
{
    /// <summary>
    /// Represents a "liveness" check. A service that fails a liveness check is considered to be unrecoverable and has to be restarted by the orchestrator.
    /// </summary>
    public const string Liveness = "liveness";

    /// <summary>
    /// Represents a "readiness" check. A service that fails a readiness check is considered to be unable to serve traffic temporarily.
    /// The orchestrator doesn't restart a service that fails this check, but stops sending traffic to it until it responds to this check positively again.
    /// </summary>
    public const string Readiness = "readiness";
}