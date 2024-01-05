namespace OIDCDemo.AuthorizationServer
{
    public class MemoryRefreshTokenStorageFactory: IRefreshTokenStorageFactory
    {
        private static readonly MemoryRefreshTokenStorage refreshTokenStorage = new();
        private static readonly MemoryRefreshTokenStorage invalidatedTokenStorage = new(1024);

        public IRefreshTokenStorage GetInvalidatedTokenStorage()
        {
            return invalidatedTokenStorage;
        }

        public IRefreshTokenStorage GetTokenStorage()
        {
            return refreshTokenStorage;
        }
    }
}
