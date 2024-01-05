using Microsoft.Extensions.DependencyInjection;
using OIDCDemo.AuthorizationServer;
using OIDCDemo.AuthorizationServer.AuthorizationClient;
using OIDCDemo.AuthorizationServer.Helpers;
using OIDCDemo.AuthorizationServer.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ICodeStorage>(services => new MemoryCodeStorage());
builder.Services.AddSingleton<IRefreshTokenStorageFactory>(services => new MemoryRefreshTokenStorageFactory());

var tokenIssuingOptions = builder.Configuration.GetSection("TokenIssuing").Get<TokenIssuingOptions>() ?? new TokenIssuingOptions();

builder.Services.AddSingleton(tokenIssuingOptions);
builder.Services.AddSingleton(JwkLoader.LoadFromDefault());
builder.Services.AddTransient<IAuthorizationClientService>(services => new FakeAuthorizationClientService(
    (id) => id == "oidc-demo-client" ? new AuthorizationClient() { 
        ClientId = "oidc-demo-client", 
        RedirectUri = "https://localhost:7100/signin-oidc"
    } : null
    ));

var app = builder.Build();

app.MapGet("/.well-known/openid-configuration", () =>
{
    return Results.File(Path.Combine(builder.Environment.ContentRootPath, "oidc-assets", ".well-known/openid-configuration.json"), contentType: "application/json");
});

app.MapGet("/.well-known/jwks.json", () =>
{
    return Results.File(Path.Combine(builder.Environment.ContentRootPath, "oidc-assets", ".well-known/jwks.json"), contentType: "application/json");
});

app.UseRouting();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
