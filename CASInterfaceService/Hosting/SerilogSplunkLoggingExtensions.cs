using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

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
}
