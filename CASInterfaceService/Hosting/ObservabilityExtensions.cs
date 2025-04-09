using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting; // Add this using directive

public static class ObservabilityExtensions
{
    private const string logOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// Configures observability instruments like logging to the web application and return an initial logger
    /// </summary>
    /// <returns>A logger that can be used during starting up the web application</returns>
    public static IServiceCollection ConfigureObservability(this IServiceCollection builder, IConfiguration configuration, IWebHostEnvironment environment)
    {
        Serilog.Debugging.SelfLog.Enable(Console.Error);
        var logger = CreateBootstrapLogger(configuration);

        var serviceName = Assembly.GetEntryAssembly()!.GetName().Name ?? throw new InvalidOperationException("Could not establish the service name");

        //builder.Host.UseSerilog((context, services, configuration) => configuration
        //            .ReadFrom.Configuration(context.Configuration)
        //            .ReadFrom.Services(services), preserveStaticLogger: true);
        builder.AddSerilog((services, config) =>
        {
            config
              .ReadFrom.Configuration(configuration)
              .ReadFrom.Services(services)
              .Enrich.WithProperty("service", serviceName)
              .WriteTo.Console(outputTemplate: logOutputTemplate);
        });

        // No other changes are needed in the file.
        //var loggerConfiguration = (LoggerConfiguration)logger;
        //loggerConfiguration
        //                .ReadFrom.Configuration(configuration)
        //                //.Enrich.WithMachineName()
        //                //.Enrich.WithProcessId()
        //                //.Enrich.WithProcessName()
        //                //.Enrich.FromLogContext()
        //                //.Enrich.WithExceptionDetails()
        //                .Enrich.WithProperty("Environment", environment.EnvironmentName)
        //                ;

        //if (appSettings.Environment.IsDevelopment())
        //{
        //    loggerConfiguration.WriteTo.Console();
        //}
        //else
        //{
        //    loggerConfiguration.WriteTo.Console(formatter: new RenderedCompactJsonFormatter());
        //    var splunkUrl = appSettings.Splunk.Url;
        //    var splunkToken = appSettings.Splunk.Token;
        //    if (string.IsNullOrWhiteSpace(splunkToken) || string.IsNullOrWhiteSpace(splunkUrl))
        //    {
        //        Log.Error($"Splunk logging sink is not configured properly, check SPLUNK_TOKEN and SPLUNK_URL env vars");
        //    }
        //    else
        //    {
        //        loggerConfiguration
        //            .WriteTo.EventCollector(
        //                splunkHost: splunkUrl,
        //                eventCollectorToken: splunkToken,
        //                messageHandler: new HttpClientHandler
        //                {
        //                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        //                },
        //                renderTemplate: false);
        //    }
        //}

        return builder;
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

    private static Serilog.ILogger CreateBootstrapLogger(IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).WriteTo.Console(outputTemplate: logOutputTemplate).CreateBootstrapLogger();
        return Log.Logger;
    }
}
