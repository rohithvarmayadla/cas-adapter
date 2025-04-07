using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System;
using System.Net.Http.Headers;
//using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
//using Newtonsoft.Json;
using Utilities;

// NOTE Coast has DEV, TEST, and PROD environments while CAS may only have TEST, and PROD (to be confirmed)
// The KeyValues from the database will authenticate https://wsgw.test.jag.gov.bc.ca but not DEV
// To get error messages returned from the service, check the pod logs for cas-interface-service
public class CasHttpClient : ICasHttpClient
{
    // TODO I remember there are some caveats to HttpClient scope and disposing, which is why the Extension DI register might be better if more HttpClients are added
    // Consider researching and implementing the commented out Extension method below, if time permits. I would assume the Extension would affectively get a HttpClient
    // from a pool of available HttpClient(s), and dispose of them properly. The downside at first glance seems to be you have multiple client api urls all cluttered 
    // in one pool of HttpClient(s)
    // There is also the topic of disposing HttpClient, touched here https://stackoverflow.com/questions/15705092/do-httpclient-and-httpclienthandler-have-to-be-disposed-between-requests
    private HttpClient _httpClient = null;

    public void Initialize(string clientId, string clientKey, string url)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("clientID", clientId);
        httpClient.DefaultRequestHeaders.Add("secret", clientKey);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //https://wsgw.test.jag.gov.bc.ca/victim/api/cas
        //httpClient.BaseAddress = new Uri(url);
        httpClient.BaseAddress = new Uri("https://wsgw.test.jag.gov.bc.ca");
        httpClient.Timeout = new TimeSpan(1, 0, 0);  // 1 hour timeout 
        _httpClient = httpClient;
    }

    public async Task<bool> ApTransaction(CasApTransactionInvoices invoices)
    {
        if (_httpClient == null)
            throw new Exception("HttpClient not initialized. Call Initialize() first.");

        // TODO check defaultDistributionAccount is not null or empty

        //var jsonRequest = JsonConvert.SerializeObject(invoices);
        var jsonRequest = invoices.ToJSONString();
        //var jsonRequest = "{\r\n \"invoiceType\": \"Standard\",\r\n \"supplierNumber\": \"2002740\",\r\n \"supplierSiteNumber\": \"001\",\r\n \"invoiceDate\": \"12-Dec-2024\",\r\n \"invoiceNumber\": \"INV-2021-002446\",\r\n \"invoiceAmount\": 1000.00,\r\n \"payGroup\": \"GEN CHQ\",\r\n \"dateInvoiceReceived\": \"12-Dec-2024\",\r\n \"dateGoodsReceived\": \"\",\r\n \"remittanceCode\": \"01\",\r\n \"specialHandling\": \"N\",\r\n \"nameLine1\": \"Ida Test Albert Test\",\r\n \"nameLine2\": \"\",\r\n \"addressLine1\": \"123 Park Road\",\r\n \"addressLine2\": \"\",\r\n \"addressLine3\": \"\",\r\n \"city\": \"Vancouver\",\r\n \"country\": \"Canada\",\r\n \"province\": \"BC\",\r\n \"postalCode\": \"T2J2Z2\",\r\n \"qualifiedReceiver\": \"team\",\r\n \"terms\": \"Immediate\",\r\n \"payAloneFlag\": \"Y\",\r\n \"paymentAdviceComments\": \"Test1\",\r\n \"remittanceMessage1\": \"Test2\",\r\n \"remittanceMessage2\": \"Test3\",\r\n \"remittanceMessage3\": \"\",\r\n \"glDate\": \"12-Dec-2024\",\r\n \"invoiceBatchName\": \"SNBATCH\",\r\n \"currencyCode\": \"CAD\",\r\n \"invoiceLineDetails\": [{\r\n   \"invoiceLineNumber\": 1,\r\n   \"invoiceLineType\": \"Item\",\r\n   \"lineCode\": \"DR\",\r\n   \"invoiceLineAmount\": 1000.00,\r\n   \"defaultDistributionAccount\": \"010.15106.12120.6038.1501300.000000.0000\",\r\n   \"description\": \"\",\r\n   \"taxClassificationCode\": \"\",\r\n   \"distributionSupplier\": \"\",\r\n   \"info1\": \"\",\r\n   \"info2\": \"\",\r\n   \"info3\": \"\"\r\n   }]\r\n}";
        //var jsonRequest = "{\r\n \"invoiceType\": \"Standard\",\r\n \"supplierNumber\": \"2002740\",\r\n \"supplierSiteNumber\": \"001\",\r\n \"invoiceDate\": \"30-Jun-2021\",\r\n \"invoiceNumber\": \"INV-2021-002446\",\r\n \"invoiceAmount\": 1000.00,\r\n \"payGroup\": \"GEN CHQ\",\r\n \"dateInvoiceReceived\": \"30-Jun-2021\",\r\n \"dateGoodsReceived\": \"\",\r\n \"remittanceCode\": \"01\",\r\n \"specialHandling\": \"N\",\r\n \"nameLine1\": \"Ida Test Albert Test\",\r\n \"nameLine2\": \"\",\r\n \"addressLine1\": \"\",\r\n \"addressLine2\": \"\",\r\n \"addressLine3\": \"\",\r\n \"city\": \"Calgary\",\r\n \"country\": \"CA\",\r\n \"province\": \"AB\",\r\n \"postalCode\": \"T2J2Z2\",\r\n \"qualifiedReceiver\": \"team\",\r\n \"terms\": \"Immediate\",\r\n \"payAloneFlag\": \"Y\",\r\n \"paymentAdviceComments\": \"\",\r\n \"remittanceMessage1\": \"\",\r\n \"remittanceMessage2\": \"\",\r\n \"remittanceMessage3\": \"\",\r\n \"glDate\": \"12-Dec-2024\",\r\n \"invoiceBatchName\": \"SNBATCH\",\r\n \"currencyCode\": \"CAD\",\r\n \"invoiceLineDetails\": [{\r\n   \"invoiceLineNumber\": 1,\r\n   \"invoiceLineType\": \"Item\",\r\n   \"lineCode\": \"DR\",\r\n   \"invoiceLineAmount\": 1000.00,\r\n   \"defaultDistributionAccount\": \"010.15106.12120.6038.1501300.000000.0000\",\r\n   \"description\": \"\",\r\n   \"taxClassificationCode\": \"\",\r\n   \"distributionSupplier\": \"\",\r\n   \"info1\": \"\",\r\n   \"info2\": \"\",\r\n   \"info3\": \"\"\r\n   }]\r\n}";
        //var jsonRequest = "";
        var url = $"{_httpClient.BaseAddress}victim/api/cas/api/CASAPTransaction";
        var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, httpContent);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Request {url} returned {response.StatusCode} status code.");
        }
        var httpResponse = await response.Content.ReadAsStringAsync();

        var jsonReader = System.Runtime.Serialization.Json.JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(httpResponse), new XmlDictionaryReaderQuotas());

        var root = XElement.Load(jsonReader);
        if (root.Element("CAS-Returned-Messages") != null)
        {
            var casReturnedMessages = root.Element("CAS-Returned-Messages");
            if (casReturnedMessages != null)
            {
                if (!(casReturnedMessages.Value.Equals("SUCCEEDED", StringComparison.InvariantCultureIgnoreCase) | casReturnedMessages.Value.Contains("Duplicate Submission")))
                    throw new Exception(casReturnedMessages.Value + "\r\n" + jsonRequest);
            }
            else
                throw new Exception(httpResponse + "\r\n" + jsonRequest);
        }
        else
            throw new Exception(httpResponse + "\r\n" + jsonRequest);

        return true;
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
