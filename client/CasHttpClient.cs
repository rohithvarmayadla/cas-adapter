namespace Client;

public class CasHttpClient(ILogger<CasHttpClient> logger) : ICasHttpClient
{
    private HttpClient _httpClient = null;
    private Model.Settings.Client _settings = null;
    private string _invoiceBaseUrl => $"{_settings.BaseUrl}/cfs/apinvoice/";
    private string _supplierBaseUrl => $"{_settings.BaseUrl}/cfs/supplier/";

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
    public async Task<HttpStatusCode> GetToken()
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
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error getting token: {response.StatusCode} - {response.Content}");
                return response.StatusCode;
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            var jo = JObject.Parse(responseBody);
            var bearerToken = jo["access_token"].ToString();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            return HttpStatusCode.OK;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting token");
            throw;
        }
    }

    public async Task<Response> Get(string url)
    {
        await GetToken()
            .ThrowIfNotSuccessful();
        var response = await _httpClient.GetAsync(url);
        var responseContent = await response.Content.ReadAsStringAsync();
        return new Response(responseContent, response.StatusCode);
    }

    public async Task<Response> Post(string url, string payload)
    {
        await GetToken()
            .ThrowIfNotSuccessful();
        var postContent = new StringContent(payload);
        var response = await _httpClient.PostAsync(url, postContent);
        var responseContent = await response.Content.ReadAsStringAsync();
        return new(responseContent, response.StatusCode);
    }

    public async Task<Response> CreateInvoice(Invoice invoice)
    {
        invoice.ThrowIfNull();

        try
        {
            var jsonString = invoice.ToJSONString();
            return await Post(_invoiceBaseUrl, jsonString);
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

    public async Task<Response> GetInvoice(string invoiceNumber, string supplierNumber, string supplierSiteCode)
    {
        invoiceNumber.ThrowIfNullOrEmpty();
        supplierNumber.ThrowIfNullOrEmpty();
        supplierSiteCode.ThrowIfNullOrEmpty();

        try
        {
            var url = $"{_invoiceBaseUrl}{invoiceNumber}/{supplierNumber}/{supplierSiteCode}";
            return await Get(url);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error searching Invoice: {invoiceNumber}, Supplier Number: {supplierNumber}, Supplier Site Code: {supplierSiteCode}.");
            return new Response(e.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Response> GetPayment(string paymentNumber, string payGroup)
    {
        paymentNumber.ThrowIfNullOrEmpty();
        payGroup.ThrowIfNullOrEmpty();

        try
        {
            var url = $"{_invoiceBaseUrl}payment/{paymentNumber}/{payGroup}";
            return await Get(url);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error searching for payment, Payment Number: {paymentNumber}, Pay Group: {payGroup}.");
            return new Response(e.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Response> GetSupplierByNumber(string supplierNumber)
    {
        supplierNumber.ThrowIfNullOrEmpty();

        try
        {
            var url = $"{_supplierBaseUrl}{supplierNumber}";
            return await Get(url);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error searching Supplier By Number and Site Code, Supplier Number : {supplierNumber}.");
            return new("Internal Error: " + e.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Response> GetSupplierByNumberAndSiteCode(string supplierNumber, string supplierSiteCode)
    {
        supplierNumber.ThrowIfNullOrEmpty();
        supplierSiteCode.ThrowIfNullOrEmpty();

        try
        {
            var url = $"{_supplierBaseUrl}{supplierNumber}/site/{supplierSiteCode}";
            return await Get(url);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error searching Supplier By Number and Site Code, Supplier Number : {supplierNumber}, Supplier Site Code: {supplierSiteCode}.");
            return new("Internal Error: " + e.Message, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Response> GetSupplierByName(string supplierName)
    {
        supplierName.ThrowIfNullOrEmpty();
        // TODO validate at least 4 characters but first if CAS does this already
        var url = $"{_settings.BaseUrl}/cfs/suppliersearch/{supplierName}";
        return await Get(url);
    }
}

public record Response(string Content, HttpStatusCode StatusCode);

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
