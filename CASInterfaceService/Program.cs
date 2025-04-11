using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Net.Http;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseUrls()
    .UseKestrel();

// add services to DI container
var services = builder.Services;
var env = builder.Environment;

// configuration binded using user secrets
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();
services.AddSingleton<IConfiguration>(configuration);

// app settings 
var appSettings = new AppSettings(configuration, env);
services.AddSingleton(appSettings);

builder.Host.UseSplunkSerilogPipe(appSettings);

// security
services.AddCorsPolicy(builder.Configuration.GetSection("cors").Get<CorsSettings>());

services.AddAuthentication()
.AddJwtBearer("jwt", options =>
{
    options.BackchannelHttpHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
    configuration.GetSection("auth:jwt").Bind(options);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
    };
    // if token does not contain a dot, it is a reference token, forward to introspection auth scheme
    options.ForwardDefaultSelector = ctx =>
    {
        var authHeader = (string)ctx.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ")) return null;
        return authHeader.Substring("Bearer ".Length).Trim().Contains('.') ? null : "introspection";
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async ctx =>
        {
            await Task.CompletedTask;
            //var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Configuration>>();
            var claims = ctx.Principal.Claims;
            foreach (var claim in claims)
            {
                //logger.LogInformation($"JWT token validated. Claim: {claim.Type}: {claim.Value}");
            }
        },
        OnAuthenticationFailed = async ctx =>
        {
            await Task.CompletedTask;
            //var clientId = oidcConfig["clientId"];
            //var issuer = oidcConfig["issuer"];
            //var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Configuration>>();
            //logger.LogError(ctx.Exception, $"JWT authentication failed: clientId={clientId}, issuer={issuer}, jwt:authority={options.Authority}");
        }
    };
})
 //reference tokens handling
 .AddOAuth2Introspection("introspection", options =>
 {
     options.EnableCaching = true;
     options.CacheDuration = TimeSpan.FromMinutes(20);
     configuration.GetSection("auth:introspection").Bind(options);
     options.Events = new OAuth2IntrospectionEvents
     {
         OnTokenValidated = async ctx =>
         {
             await Task.CompletedTask;
             var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger>();
             var userInfo = ctx.Principal?.FindFirst("userInfo");
             logger.LogDebug("{0}", userInfo);
         },
         OnAuthenticationFailed = async ctx =>
         {
             await Task.CompletedTask;
             //var logger = ctx.HttpContext.RequestServices.GetRequiredService<ITelemetryProvider>().Get<JwtBearerEvents>();
             //logger.LogError(ctx?.Result?.Failure, "Introspection authantication failed");
         }
     };
});

services.AddAuthorization(options =>
{
    options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
    {
        policy
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes("jwt");
        //.RequireClaim("scope", appSettings.Auth.Jwt.Scope);
    });

    options.DefaultPolicy = options.GetPolicy(JwtBearerDefaults.AuthenticationScheme) ?? null!;
});

services.AddSerilog(appSettings);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// security
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();
app.UseDisableHttpVerbsMiddleware(app.Configuration.GetValue("DisabledHttpVerbs", string.Empty));
app.UseCsp();
app.UseSecurityHeaders();
app.UseCors();

app.UseLoggingMiddleware();
app.Run();