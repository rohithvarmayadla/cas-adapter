using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseUrls()
    .UseKestrel(options =>
    {
        options.Listen(IPAddress.Any, 8081);
    });

// add services to DI container
var services = builder.Services;
var env = builder.Environment;

// configuration binded using user secrets
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();
services.AddSingleton<IConfiguration>(configuration);

// app settings 
var appSettings = new AppSettings(configuration, env);
services.AddSingleton(appSettings);

builder.Host.UseSplunkSerilogPipe(appSettings);

services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;

});
services.AddSerilog(appSettings);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();
app.MapControllerRoute("default", "{controller}/{action=Index}/{id?}");
app.UseAuthorization();
app.UseLoggingMiddleware();
app.Run();