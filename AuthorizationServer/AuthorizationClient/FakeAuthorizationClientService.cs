namespace OIDCDemo.AuthorizationServer.AuthorizationClient
{
    public class FakeAuthorizationClientService : IAuthorizationClientService
    {
        private readonly Func<string, AuthorizationClient?>? findByIdFunc = null;

        public FakeAuthorizationClientService(Func<string, AuthorizationClient?>? findByIdFunc)
        {
            this.findByIdFunc = findByIdFunc;
        }

        public AuthorizationClient? FindById(string id)
        {
            if (findByIdFunc != null)
            {
                return findByIdFunc(id);
            }

            return null;
        }
    }
}
