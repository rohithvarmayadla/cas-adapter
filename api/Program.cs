var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseUrls()
    .UseKestrel();

// add services to DI container
var services = builder.Services;
var env = builder.Environment;

var appSettings = services.AddAppSettings(env);
services.AddTransient<ICasHttpClient, CasHttpClient>();

builder.Host.UseLogging(appSettings);

services.AddCorsPolicy(builder.Configuration.GetSection("cors").Get<CorsSettings>());
services.AddAuthentication(appSettings);
services.AddAuthorization(appSettings);
services.AddHealthChecks();
services.AddLogging(appSettings);
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();
app.MapHealthChecks();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers()
    .RequireAuthorization();
if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDisableHttpVerbsMiddleware(app.Configuration.GetValue("DisabledHttpVerbs", string.Empty));
app.UseCsp();
app.UseSecurityHeaders();
app.UseCors();
app.UseLoggingMiddleware();

app.Run();