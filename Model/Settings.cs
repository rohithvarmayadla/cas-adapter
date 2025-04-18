namespace Model.Settings;

public class Splunk 
{
    public string Url { get; set; }
    public string Token { get; set; }
}

public class Auth
{
    public JwtSection Jwt { get; set; }
    //public Oidc Oidc { get; set; }

    public class JwtSection
    {
        public string Authority { get; set; }
        public string Scope { get; set; }
    }
}

public class Client
{
    public string Id { get; set; }
    public string Secret { get; set; }
    public string BaseUrl { get; set; }
    public string TokenUrl { get; set; }
}
