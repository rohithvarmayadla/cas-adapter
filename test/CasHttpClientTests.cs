public class CasHttpClientTests(ICasHttpClient casHttpClient, AppSettings appSettings)
{
    [Fact]
    public async Task Get_Token_Success()
    {
        casHttpClient.Initialize(appSettings.Client);

        await casHttpClient.GetToken();
    }
}
