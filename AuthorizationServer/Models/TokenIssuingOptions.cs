using Microsoft.IdentityModel.Tokens;

namespace OIDCDemo.AuthorizationServer.Models
{
    public class TokenIssuingOptions
    {
        public string Issuer { get; set; } = string.Empty; // this server's Issuer ID, must be an HTTPS URL
        public int IdTokenExpirySeconds { get; set; } = 60 * 20;
        public int AccessTokenExpirySeconds { get; set; } = 60 * 5;
        public int RefreshTokenExpirySeconds { get; set; } = 60 * 24 * 7;
    }
}
