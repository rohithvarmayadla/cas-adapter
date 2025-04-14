using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace api;

public class AppSettings
{
    public readonly Splunk Splunk;
    public readonly IConfiguration Configuration;
    public readonly IWebHostEnvironment Environment;
    public readonly string LoggingOutputFormat;

    public AppSettings(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
        Splunk = new Splunk
        {
            Url = configuration["SPLUNK_URL"],
            Token = configuration["SPLUNK_TOKEN"]
        };
        LoggingOutputFormat = configuration["LOGGING_OUTPUT"];
    }
}

public class Splunk 
{
    public string Url { get; set; }
    public string Token { get; set; }
}
