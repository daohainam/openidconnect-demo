using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OIDCDemo.Client;
using OIDCDemo.Client.Data;
using OIDCDemo.Client.Helpers;

IdentityModelEventSource.ShowPII = true; // enable detailed logs

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var options = builder.Configuration.GetSection("OpenIdConnect").Get<ClientOptions>() ?? throw new Exception("Could not get ClientOptions");

builder.Services.AddDefaultIdentity<IdentityUser>(
    options => options.SignIn.RequireConfirmedAccount = true
    )
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(options =>
{
})
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, openIdOptions =>
{
    openIdOptions.ClientId = options.ClientId;
    openIdOptions.Authority = options.Issuer;
    openIdOptions.ResponseType = OpenIdConnectResponseType.Code;
    openIdOptions.CallbackPath = options.CallbackPath;
    openIdOptions.SaveTokens = true;
    openIdOptions.AccessDeniedPath = options.AccessDeniedPath;

    openIdOptions.GetClaimsFromUserInfoEndpoint = false; // we will change this to true when we implement user-info endpoint

    foreach (var scope in options.Scope.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
        openIdOptions.Scope.Add(scope);
    }

    openIdOptions.TokenValidationParameters.ValidIssuer = options.Issuer;
    openIdOptions.TokenValidationParameters.ValidAudience = options.ClientId;
    openIdOptions.TokenValidationParameters.ValidAlgorithms = new[] { "RS256" };
    openIdOptions.TokenValidationParameters.IssuerSigningKey = JwkLoader.LoadFromPublic();

    openIdOptions.Events.OnAuthorizationCodeReceived = (context) =>
    {
        Console.WriteLine($"authorization_code: {context.ProtocolMessage.Code}");

        return Task.CompletedTask;
    };

    openIdOptions.Events.OnTokenResponseReceived = (context) =>
    {
        Console.WriteLine($"OnTokenResponseReceived.access_token: {context.TokenEndpointResponse.AccessToken}");
        Console.WriteLine($"OnTokenResponseReceived.refresh_token: {context.TokenEndpointResponse.RefreshToken}");

        return Task.CompletedTask;
    };

    openIdOptions.Events.OnTokenValidated = (context) =>
    {
        Console.WriteLine($"OnTokenValidated.access_token: {context.TokenEndpointResponse?.AccessToken}");
        Console.WriteLine($"OnTokenValidated.refresh_token: {context.TokenEndpointResponse?.RefreshToken}");

        return Task.CompletedTask;
    };

    openIdOptions.Events.OnTicketReceived = (context) =>
    {
        return Task.CompletedTask;
    };

});

builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
