namespace OIDCDemo.AuthorizationServer.Models
{
    public struct Tokens
    {
        public required string AccessToken { get; set; }
        public required string TokenType { get; set; }
        public required string IdToken { get; set; }
        public required DateTime ExpiryTime { get; set; }
    }
}
