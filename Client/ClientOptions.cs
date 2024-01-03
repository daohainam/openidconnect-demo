namespace OIDCDemo.Client
{
    public class ClientOptions
    {
        public string ClientId { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public string CallbackPath { get; set; } = "/signin-oidc";
        public string AccessDeniedPath { get; set; } = "/access-denied";
    }
}
