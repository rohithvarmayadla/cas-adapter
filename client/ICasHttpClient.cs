namespace Utilities;

public interface ICasHttpClient
{
    void Initialize(Client settings);
    Task<string> GetToken();
    Task<Tuple<string, HttpStatusCode>> ApTransaction(CasApTransaction invoices);
}