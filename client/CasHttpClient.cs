namespace Client;

public class CasHttpClient(ILogger<CasHttpClient> logger) : ICasHttpClient
{
    private HttpClient _httpClient = null;
    private Model.Settings.Client _settings = null;

    // TODO this will be removed when "Access Token Management" ticket is completed
    public void Initialize(Model.Settings.Client settings)
    {
        _settings = settings;

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.BaseAddress = new Uri(settings.BaseUrl);
        httpClient.Timeout = new TimeSpan(1, 0, 0);  // 1 hour timeout 
        _httpClient = httpClient;
    }

    // get authentication bearer token for subsequent requests
    public async Task GetToken()
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", _settings.Id, _settings.Secret))));

            var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenUrl);
            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            request.Content = new FormUrlEncodedContent(formData);

            var response = await _httpClient.SendAsync(request);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            var jo = JObject.Parse(responseBody);
            var bearerToken = jo["access_token"].ToString();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting token");
            throw;
        }
    }

    public async Task<Tuple<string, HttpStatusCode>> GenerateInvoice(CasApTransaction invoice)
    {
        invoice.ThrowIfNull();

        try
        {
            var url = _settings.BaseUrl + "/cfs/apinvoice/";
            await GetToken();
            var jsonString = invoice.ToJSONString();
            var postContent = new StringContent(jsonString);
            var response = await _httpClient.PostAsync(url, postContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            return new(responseContent, response.StatusCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error generating invoice: {invoice.InvoiceNumber}.");
            dynamic errorObject = new JObject();
            errorObject.invoice_number = invoice.InvoiceNumber;
            errorObject.CAS_Returned_Messages = "Internal Error: " + e.Message;
            return new (JsonConvert.SerializeObject(errorObject), HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Tuple<string, HttpStatusCode>> SearchInvoice(string invoiceNumber, string supplierNumber, string supplierSiteCode)
    {
        invoiceNumber.ThrowIfNullOrEmpty();
        supplierNumber.ThrowIfNullOrEmpty();
        supplierSiteCode.ThrowIfNullOrEmpty();

        try
        {
            var url = $"{_settings.BaseUrl}/cfs/apinvoice/{invoiceNumber}/{supplierNumber}/{supplierSiteCode}";
            await GetToken();
            var response = await _httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            return new(responseContent, response.StatusCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error searching Invoice: {invoiceNumber}, Supplier Number: {supplierNumber}, Supplier Site Code: {supplierSiteCode}.");
            return new("Internal Error: " + e.Message, HttpStatusCode.InternalServerError);
        }
    }
}

//public static class CasHttpClientExtensions
//{
//    public static IServiceCollection AddHttpClient(this IServiceCollection services, string clientKey, string clientId, string url)
//    {
//        // TODO add http client request logger e.g. interceptor or decorator
//        //Log.AppendLine("Sending Json: " + jsonRequest.ToString());
//        var httpClient = new HttpClient();
//        httpClient.DefaultRequestHeaders.Add("clientID", clientId);
//        httpClient.DefaultRequestHeaders.Add("secret", clientKey);
//        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//        httpClient.BaseAddress = new Uri(url);
//        httpClient.Timeout = new TimeSpan(1, 0, 0);  // 1 hour timeout 

//        services.AddHttpClient<ICasHttpClient, CasHttpClient>(serviceProvider => new CasHttpClient(httpClient));
//        return services;
//    }
//}
