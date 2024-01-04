using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OIDCDemo.ResourceServer;
using OIDCDemo.ResourceServer.Helpers;

var builder = WebApplication.CreateBuilder(args);

var jwtBearerOptions = builder.Configuration.GetSection("JWTBearer").Get<JWTBearerOptions>() ?? throw new Exception("Could not get JWTBearerOptions");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.ClaimsIssuer = options.ClaimsIssuer;

    options.TokenHandlers.Add(new MyJsonWebTokenHandler());

    options.TokenValidationParameters.ValidIssuer = jwtBearerOptions.Issuer;
    options.TokenValidationParameters.ValidAudience = jwtBearerOptions.ClientId;
    options.TokenValidationParameters.ValidAlgorithms = new[] { "RS256" };
    options.TokenValidationParameters.IssuerSigningKey = JwkLoader.LoadFromPublic();
});
builder.Services.AddAuthorization(options =>
{    
    options.AddPolicy("read", policy => policy.RequireClaim("read"));
    options.AddPolicy("write", policy => policy.RequireClaim("write"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
