namespace Api;

public class AppSettings
{
    public readonly Splunk Splunk;
    public readonly IConfiguration Configuration;
    public readonly IWebHostEnvironment Environment;
    public readonly Auth Auth;
    public readonly Model.Settings.Client Client;

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
        Client = new Model.Settings.Client
        {
            Id = configuration["ClientId"],
            Secret = configuration["ClientKey"],
            BaseUrl = configuration["BaseUrl"],
            TokenUrl = configuration["TokenUrl"]
        };
    }
}

public static class AppSettingsExtensions
{
    public static AppSettings AddAppSettings(this IServiceCollection services, IWebHostEnvironment environment)
    {
        // configuration binded using user secrets
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // app settings 
        var appSettings = new AppSettings(configuration, environment);
        services.AddSingleton(appSettings);
        return appSettings;
    }
}