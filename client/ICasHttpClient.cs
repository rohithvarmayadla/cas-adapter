namespace Utilities;

public interface ICasHttpClient
{
    void Initialize(string clientId, string clientKey, string url);
    Task<bool> ApTransaction(CasApTransactionInvoices invoices);
}