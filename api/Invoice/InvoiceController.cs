namespace Api;

[Route("api/[controller]")]
[ApiController]
public class InvoiceController : Controller
{
    private readonly ICasHttpClient _casHttpClient;

    public InvoiceController(AppSettings appSettings, ICasHttpClient casHttpClient)
    {
        _casHttpClient = casHttpClient;

        if (string.IsNullOrEmpty(appSettings.Client?.Id) || string.IsNullOrEmpty(appSettings.Client.Secret) || string.IsNullOrEmpty(appSettings.Client.BaseUrl) || string.IsNullOrEmpty(appSettings.Client.TokenUrl))
        {
            throw new ArgumentNullException("Client is not configured. Check your user secrets, appsettings, and environment variables.");
        }

        _casHttpClient.Initialize(appSettings.Client);
    }

    [HttpPost]
    public async Task<IActionResult> Generate([FromBody] CasApTransaction invoice)
    {
        if (invoice == null)
        {
            return BadRequest("Invoice data is required.");
        }

        (var result, var statusCode) = await _casHttpClient.CreateInvoice(invoice);

        return StatusCode((int)statusCode, new JsonResult(result).Value);
    }

    [HttpGet("{invoiceNumber}/{supplierNumber}/{supplierSiteCode}")]
    public async Task<IActionResult> Search(string invoiceNumber, string supplierNumber, string supplierSiteCode)
    {
        if (string.IsNullOrEmpty(invoiceNumber) || string.IsNullOrEmpty(supplierNumber) || string.IsNullOrEmpty(supplierSiteCode))
        {
            return BadRequest("Invoice number, supplier number, and supplier site code are required.");
        }

        (var result, var statusCode) = await _casHttpClient.GetInvoice(invoiceNumber, supplierNumber, supplierSiteCode);

        return StatusCode((int)statusCode, new JsonResult(result).Value);
    }
}
