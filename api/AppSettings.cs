namespace Api;

public class AppSettings
{
    public readonly Splunk Splunk;
    public readonly IConfiguration Configuration;
    public readonly IWebHostEnvironment Environment;
    public readonly Auth Auth;
    public readonly Client Client;

    public AppSettings(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
        Splunk = new Splunk
        {
            Url = configuration["SPLUNK_URL"],
            Token = configuration["SPLUNK_TOKEN"]
        };
        Auth = configuration.GetSection("auth").Get<Auth>();
        Client = new Client
        {
            Id = configuration["ClientId"],
            Secret = configuration["ClientKey"],
            BaseUrl = configuration["BaseUrl"],
            TokenUrl = configuration["TokenUrl"]
        };
    }
}