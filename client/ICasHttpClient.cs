namespace Utilities;

public interface ICasHttpClient
{
    void Initialize(string clientId, string clientKey, string url);
    Task<string> GetToken();
    Task<bool> ApTransaction(CasApTransaction invoices);
}