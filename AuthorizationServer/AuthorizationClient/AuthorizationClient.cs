using Microsoft.AspNetCore.Mvc;

namespace OIDCDemo.AuthorizationServer.AuthorizationClient
{
    public class AuthorizationClient
    {
        public string ClientId { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
    }
}
