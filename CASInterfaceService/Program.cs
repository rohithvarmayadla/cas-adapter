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

builder.Host.UseLogging(appSettings);

// security
services.AddCorsPolicy(builder.Configuration.GetSection("cors").Get<CorsSettings>());
services.AddAuthentication(appSettings);
services.AddAuthorization(appSettings);

services.AddLogging(appSettings);

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