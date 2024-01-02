using System.Text.Json.Serialization;

namespace OIDCDemo.AuthorizationServer
{
    public interface ICodeStorage
    {
        bool TryAddCode(string code, DateTime expiryTime);
        bool TryGetToken(string code, out DateTime expiryTime);
    }


}
