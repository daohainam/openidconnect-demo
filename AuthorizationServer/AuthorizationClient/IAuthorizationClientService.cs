namespace OIDCDemo.AuthorizationServer.AuthorizationClient
{
    public interface IAuthorizationClientService
    {
        AuthorizationClient? FindById(string id);
    }
}
