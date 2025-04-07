using Microsoft.Extensions.DependencyInjection;
using Utilities;

public class Startup
{
    /// <summary>
    /// Register dependencies needed for xunit tests
    /// NOTE to register dependencies used by making calls from HttpClient, use CustomWebApplicationFactory
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
    {
        //var configuration = new ConfigurationBuilder()
        //    .AddUserSecrets<Gov.Cscp.VictimServices.Public.Startup>()
        //    .AddEnvironmentVariables()
        //    .Build();
        //services.AddSingleton<IConfiguration>(configuration);

        //services.AddAutoMapperMappings();

        //services.AddHandlers();
        //services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<InvoiceHandlers>());

        //services.AddTransient<FakeHandlers>();
        //services.AddTransient<IFakeRepository, FakeRepository>();

        services.AddSingleton<ICasHttpClient, CasHttpClient>();
    }
}
