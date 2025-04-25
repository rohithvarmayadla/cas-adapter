namespace Api;

public static class SerilogSplunkLoggingExtensions
{
    /// <summary>
    /// Configures observability instruments like logging to the web application and return an initial logger
    /// </summary>
    /// <returns>A logger that can be used during starting up the web application</returns>
    public static IServiceCollection AddLogging(this IServiceCollection services, AppSettings appSettings)
    {
        Serilog.Debugging.SelfLog.Enable(Console.Error);
        CreateBootstrapLogger(appSettings);
        var serviceName = Assembly.GetEntryAssembly()!.GetName().Name ?? throw new InvalidOperationException("Could not establish the service name");

        services.AddSerilog((services, config) =>
        {
            config
              .ReadFrom.Configuration(appSettings.Configuration)
              .ReadFrom.Services(services)
              .Enrich.WithProperty("service", serviceName);
        });

        return services;
    }

    public static void UseLogging(this ConfigureHostBuilder builder, AppSettings appSettings)
    {
        builder.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(appSettings.Configuration)
                .Enrich.WithProperty("Environment", appSettings.Environment.EnvironmentName);

            if (!appSettings.Environment.IsDevelopment()) 
            {
                loggerConfiguration.WriteTo.Console(formatter: new RenderedCompactJsonFormatter());
                var splunkUrl = appSettings.Splunk.Url;
                var splunkToken = appSettings.Splunk.Token;
                if (string.IsNullOrWhiteSpace(splunkToken) || string.IsNullOrWhiteSpace(splunkUrl))
                {
                    Log.Error($"Splunk logging sink is not configured properly, check SPLUNK_TOKEN and SPLUNK_URL env vars");
                }
                else
                {
                    loggerConfiguration
                        .WriteTo.EventCollector(
                            splunkHost: splunkUrl,
                            eventCollectorToken: splunkToken,
                            messageHandler: new HttpClientHandler
                            {
                                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                            },
                            renderTemplate: false);
                }
            }
        });
    }

    /// <summary>
    /// Adds observability instruments like logging to the web application's middleware pipelines
    /// </summary>
    public static void UseLoggingMiddleware(this WebApplication webApplication)
    {
        webApplication.UseSerilogRequestLogging(opts =>
        {
            opts.GetLevel = GetLevel;
            opts.IncludeQueryInRequestPath = true;
            opts.EnrichDiagnosticContext = (diagCtx, httpCtx) =>
            {
                diagCtx.Set("User", httpCtx.User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty);
            };
        });
    }

    private static void CreateBootstrapLogger(AppSettings appSettings)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(appSettings.Configuration)
            .WriteTo.Console()
            .CreateBootstrapLogger();
    }

    private static bool IsHealthCheckEndpoint(HttpContext ctx)
    {
        var endpoint = ctx.GetEndpoint();
        if (endpoint is object) // same as !(endpoint is null)
        {
            return string.Equals(
                endpoint.DisplayName,
                "Health checks",
                StringComparison.Ordinal);
        }
        // No endpoint, so not a health check endpoint
        return false;
    }

    // summary logs for health check requests use a Verbose level, while errors use Error and other requests use Information
    private static LogEventLevel GetLevel(HttpContext ctx, double _, Exception ex) =>
        ex != null
            ? LogEventLevel.Error
            : ctx.Response.StatusCode > 499
                ? LogEventLevel.Error
                : IsHealthCheckEndpoint(ctx) // Not an error, check if it was a health check
                    ? LogEventLevel.Verbose // Was a health check, use Verbose
                    : LogEventLevel.Information;
}
