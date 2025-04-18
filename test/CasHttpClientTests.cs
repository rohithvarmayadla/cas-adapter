public class CasHttpClientTests(ICasHttpClient casHttpClient, AppSettings appSettings)
{
    [Fact]
    public async Task Get_Token_Success()
    {
        casHttpClient.Initialize(appSettings.Client);

        var token = await casHttpClient.GetToken();

        Assert.NotNull(token);
    }
}
