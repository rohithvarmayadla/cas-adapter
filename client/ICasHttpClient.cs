namespace Client;

public interface ICasHttpClient
{
    void Initialize(Model.Settings.Client settings);
    Task GetToken();
    Task<Tuple<string, HttpStatusCode>> GenerateInvoice(CasApTransaction invoices);
    Task<Tuple<string, HttpStatusCode>> SearchInvoice(string invoiceNumber, string supplierNumber, string supplierSiteCode);
}