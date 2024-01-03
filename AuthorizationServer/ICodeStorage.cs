using System.Text.Json.Serialization;

namespace OIDCDemo.AuthorizationServer
{
    public interface ICodeStorage
    {
        bool TryAddCode(string code, CodeStorageValue codeStorageValue);
        bool TryGetToken(string code, out CodeStorageValue? codeStorageValue);
    }

    public class CodeStorageValue
    {
        public required string ClientId { get; set; }
        public required string Code { get; set; }
        public required DateTime ExpiryTime { get; set; }
        public required string OriginalRedirectUri { get; set; }
        public required string User { get; set; }
        public required string Nonce { get; set; }
    }
}
