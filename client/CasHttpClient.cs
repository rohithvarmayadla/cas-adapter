using Newtonsoft.Json;

namespace Client;

// NOTE Coast has DEV, TEST, and PROD environments while CAS may only have TEST, and PROD (to be confirmed)
// The KeyValues from the database will authenticate https://wsgw.test.jag.gov.bc.ca but not DEV
// To get error messages returned from the service, check the pod logs for cas-interface-service
public class CasHttpClient(ILogger<CasHttpClient> logger) : ICasHttpClient
{
    // TODO I remember there are some caveats to HttpClient scope and disposing, which is why the Extension DI register might be better if more HttpClients are added
    // Consider researching and implementing the commented out Extension method below, if time permits. I would assume the Extension would affectively get a HttpClient
    // from a pool of available HttpClient(s), and dispose of them properly. The downside at first glance seems to be you have multiple client api urls all cluttered 
    // in one pool of HttpClient(s)
    // There is also the topic of disposing HttpClient, touched here https://stackoverflow.com/questions/15705092/do-httpclient-and-httpclienthandler-have-to-be-disposed-between-requests
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
    public async Task<string> GetToken()
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
            return jo["access_token"].ToString();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting token");
            throw;
        }
    }

    public async Task<Tuple<string, HttpStatusCode>> ApTransaction(CasApTransaction invoices)
    {
        try
        {
            var URL = _settings.BaseUrl + "/cfs/apinvoice/";
            var responseToken = await GetToken();

            using (var packageClient = new HttpClient())
            {
                packageClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", responseToken);
                var jsonString = invoices.ToJSONString();
                var postContent = new StringContent(jsonString);
                var packageResult = await packageClient.PostAsync(URL, postContent);
                var outputMessage = await packageResult.Content.ReadAsStringAsync();
                var httpStatusCode = packageResult.StatusCode;
                return new(outputMessage, httpStatusCode);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error generating invoice: {invoices.InvoiceNumber}");
            dynamic errorObject = new JObject();
            errorObject.invoice_number = invoices.InvoiceNumber;
            errorObject.CAS_Returned_Messages = "Internal Error: " + e.Message;
            return new (JsonConvert.SerializeObject(errorObject), HttpStatusCode.InternalServerError);
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
