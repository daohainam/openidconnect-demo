namespace OIDCDemo.AuthorizationServer
{
    public interface IRefreshTokenStorageFactory
    {
        IRefreshTokenStorage GetTokenStorage();
        IRefreshTokenStorage GetInvalidatedTokenStorage();
    }
}
