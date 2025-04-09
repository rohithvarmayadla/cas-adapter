using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using System;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public static class ObservabilityExtensions
{
    private const string logOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// Configures observability instruments like logging to the web application and return an initial logger
    /// </summary>
    /// <returns>A logger that can be used during starting up the web application</returns>
    public static ILogger ConfigureWebApplicationObservability(this WebApplicationBuilder builder)
    {
        Serilog.Debugging.SelfLog.Enable(Console.Error);
        var logger = CreateBootstrapLogger(builder.Configuration);

        var serviceName = Assembly.GetEntryAssembly()!.GetName().Name ?? throw new InvalidOperationException("Could not establish the service name");

        //builder.Host.UseSerilog((context, services, configuration) => configuration
        //            .ReadFrom.Configuration(context.Configuration)
        //            .ReadFrom.Services(services), preserveStaticLogger: true);
        builder.Services.AddSerilog((services, config) =>
        {
            config
              .ReadFrom.Configuration(builder.Configuration)
              .ReadFrom.Services(services)
              .Enrich.WithProperty("service", serviceName)
              .WriteTo.Console(outputTemplate: logOutputTemplate);
        });

        var loggerConfiguration = (LoggerConfiguration)Log.Logger;
        loggerConfiguration
                        .ReadFrom.Configuration(builder.Configuration)
                        //.Enrich.WithMachineName()
                        //.Enrich.WithProcessId()
                        //.Enrich.WithProcessName()
                        //.Enrich.FromLogContext()
                        //.Enrich.WithExceptionDetails()
                        .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName);

        if (builder.Environment.IsDevelopment())
        {
            loggerConfiguration.WriteTo.Console();
        }
        else
        {
            loggerConfiguration.WriteTo.Console(formatter: new RenderedCompactJsonFormatter());
            //var splunkUrl = builder.Configuration.GetSplunkUrl();
            //var splunkToken = builder.Configuration.GetSplunkToken();
            //if (string.IsNullOrWhiteSpace(splunkToken) || string.IsNullOrWhiteSpace(splunkUrl))
            //{
            //    Log.Error($"Splunk logging sink is not configured properly, check SPLUNK_TOKEN and SPLUNK_URL env vars");
            //}
            //else
            //{
            //    loggerConfiguration
            //        .WriteTo.EventCollector(
            //            splunkHost: splunkUrl,
            //            eventCollectorToken: splunkToken,
            //            messageHandler: new HttpClientHandler
            //            {
            //                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            //            },
            //            renderTemplate: false);
            //}
        }

        return logger;
    }

    /// <summary>
    /// Adds observability instruments like logging to the web application's middleware pipelines
    /// </summary>
    public static void UseObservabilityMiddleware(this WebApplication webApplication)
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

    private static ILogger CreateBootstrapLogger(IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).WriteTo.Console(outputTemplate: logOutputTemplate).CreateBootstrapLogger();
        return (ILogger)Log.Logger;
    }
}
