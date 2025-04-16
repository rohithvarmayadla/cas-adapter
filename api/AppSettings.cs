namespace Api;

public class AppSettings
{
    public readonly Splunk Splunk;
    public readonly IConfiguration Configuration;
    public readonly IWebHostEnvironment Environment;
    public readonly Auth Auth;

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
    }
}

public class Splunk 
{
    public string Url { get; set; }
    public string Token { get; set; }
}

public class Auth
{
    public JwtSection Jwt { get; set; }
    //public Oidc Oidc { get; set; }

    public class JwtSection
    {
        public string Authority { get; set; }
        public string Scope { get; set; }
    }
}
