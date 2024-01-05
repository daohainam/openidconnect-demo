namespace OIDCDemo.AuthorizationServer
{
    public interface IRefreshTokenStorage
    {
        bool TryAddToken(string token);
        bool Contains(string token);
    }
}
