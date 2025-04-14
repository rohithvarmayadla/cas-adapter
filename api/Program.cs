using api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

services.AddCorsPolicy(builder.Configuration.GetSection("cors").Get<CorsSettings>());

services.AddSerilog(appSettings);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();
app.MapControllerRoute("default", "{controller}/{action=Index}/{id?}");

// security
app.UseDisableHttpVerbsMiddleware(app.Configuration.GetValue("DisabledHttpVerbs", string.Empty));
app.UseCsp();
app.UseSecurityHeaders();
app.UseCors();

app.UseLoggingMiddleware();
app.Run();