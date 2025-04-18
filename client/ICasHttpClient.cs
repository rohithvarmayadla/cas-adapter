namespace Client;

public interface ICasHttpClient
{
    void Initialize(Model.Settings.Client settings);
    Task<string> GetToken();
    Task<Tuple<string, HttpStatusCode>> ApTransaction(CasApTransaction invoices);
}