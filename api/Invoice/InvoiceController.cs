namespace Api;

[Route("api/[controller]")]
[ApiController]
public class InvoiceController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ICasHttpClient _casHttpClient;

    public InvoiceController(IConfiguration configuration, ICasHttpClient casHttpClient)
    {
        _configuration = configuration;
        _casHttpClient = casHttpClient;
    }

    [HttpPost]
    public async Task<IActionResult> Generate([FromBody] CasApTransaction invoice)
    {
        if (invoice == null)
        {
            return BadRequest("Invoice data is required.");
        }
        var clientId = _configuration["ClientId"];
        var clientKey = _configuration["ClientKey"];
        _casHttpClient.Initialize(clientId, clientKey, "https://wsgw.test.jag.gov.bc.ca/victim/api/cas");
        await _casHttpClient.ApTransaction(invoice);
        return Ok("Invoice sent successfully.");
    }
}
