public class CasHttpClientTests(ICasHttpClient casHttpClient, IConfiguration configuration)
{
    [Fact]
    public async Task Get_Token_Success()
    {
        var clientId = configuration["ClientId"];
        var clientKey = configuration["ClientKey"];
        casHttpClient.Initialize(clientId, clientKey, "https://wsgw.test.jag.gov.bc.ca/victim/api/castrain");

        var token = await casHttpClient.GetToken();

        Assert.NotNull(token);
    }
}
