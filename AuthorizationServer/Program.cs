using OIDCDemo.AuthorizationServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ICodeStorage>(services => new MemoryCodeStorage());

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
