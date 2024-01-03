﻿using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OIDCDemo.AuthorizationServer.AuthorizationClient;
using OIDCDemo.AuthorizationServer.Helpers;
using OIDCDemo.AuthorizationServer.Models;
using System.CodeDom.Compiler;
using System.Net.Http;
using System.Security.Claims;

namespace OIDCDemo.AuthorizationServer.Controllers
{
    public class AuthorizeController : Controller
    {
        public const int TokenResponseValidSeconds = 1200;
        public const int CodeResponseValidSeconds = 60 * 5;

        private readonly ILogger<AuthorizeController> logger;
        private readonly TokenIssuingOptions tokenIssuingOptions;
        private readonly JsonWebKey jsonWebKey;
        private readonly ICodeStorage codeStorage;
        private readonly IAuthorizationClientService authorizationClientService;

        public AuthorizeController(
            TokenIssuingOptions tokenIssuingOptions,
            JsonWebKey jsonWebKey,
            ICodeStorage codeStorage, 
            IAuthorizationClientService authorizationClientService, 
            ILogger<AuthorizeController> logger)
        {
            this.tokenIssuingOptions = tokenIssuingOptions;
            this.jsonWebKey = jsonWebKey;
            this.codeStorage = codeStorage;
            this.authorizationClientService = authorizationClientService;
            this.logger = logger;
        }

        public IActionResult Index(AuthenticationRequestModel authenticateRequest)
        {
            ValidateAuthenticateRequestModel(authenticateRequest);
            return View(authenticateRequest);
        }

        [HttpPost]
        public IActionResult AuthorizeAsync(AuthenticationRequestModel authenticateRequest, string user, string[] scopes)
        {
            ArgumentNullException.ThrowIfNull(nameof(authenticateRequest));
            ArgumentException.ThrowIfNullOrEmpty(nameof(user));
            ArgumentNullException.ThrowIfNull(scopes);

            ValidateAuthenticateRequestModel(authenticateRequest);

            var client = authorizationClientService.FindById(authenticateRequest.ClientId);
            if (client == null)
            {
                return BadRequest("Invalid client_id");
            }

            if (client.RedirectUri != authenticateRequest.RedirectUri)
            {
                return BadRequest("Invalid redirect_uri");
            }

            string code = GenerateAuthenticationCode();
            if (!codeStorage.TryAddCode(code, new CodeStorageValue() {
                Code = code,
                ClientId = authenticateRequest.ClientId,
                OriginalRedirectUri = authenticateRequest.RedirectUri,
                ExpiryTime = DateTime.Now.AddSeconds(CodeResponseValidSeconds),
                Nonce = authenticateRequest.Nonce,
                User = user
            }
            )) 
            {
                throw new Exception("Error storing code"); 
            }

            var codeFlowModel = BuildCodeFlowResponseModel(authenticateRequest, code);

            string viewName = "SubmitForm"; // we can change to another view if we need to support response_modes other than form_post

            logger.LogInformation("New authentication code issued: {c}", code);

            return View(viewName, new CodeFlowResponseViewModel()
            {
                Code = codeFlowModel.Code,
                RedirectUri = authenticateRequest.RedirectUri,
                State = codeFlowModel.State,
            });
        }

        [HttpPost("/token")]
        [ResponseCache(NoStore = true)] // https://openid.net/specs/openid-connect-core-1_0.html#TokenEndpoint
        public IActionResult GetTokenAsync(string grant_type, string code, string redirect_uri)
        {
            if (grant_type != "authorization_code")
            {
                return BadRequest();
            }

            if (!codeStorage.TryGetToken(code, out var codeStorageValue) || codeStorageValue == null)
            {
                return BadRequest();
            }

            if (codeStorageValue.OriginalRedirectUri != redirect_uri)
            {
                return BadRequest();
            }

            var client = authorizationClientService.FindById(codeStorageValue.ClientId);
            if (client == null)
            {
                return BadRequest();
            }

            // refresh token can actually be implemented as a JWT or an unique id string

            var result = new AuthenticationResponseModel() { 
                AccessToken = GenerateAccessToken(codeStorageValue.User, client.ClientId, codeStorageValue.Nonce, jsonWebKey),
                IdToken = GenerateIdToken(codeStorageValue.User, client.ClientId, codeStorageValue.Nonce, jsonWebKey),
                TokenType = "Bearer",
                RefreshToken = GenerateRefreshToken(), 
                ExpiresIn = TokenResponseValidSeconds // valid in 20 minutes
            };

            return Json(result);
        }

        private string GenerateRefreshToken()
        {
            return string.Empty;
        }

        private string GenerateIdToken(string userId, string audience, string nonce, JsonWebKey jsonWebKey)
        {
            // https://openid.net/specs/openid-connect-core-1_0.html#IDToken

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId)
            };

            var idToken = JwtGenerator.GenerateJWTToken(
                tokenIssuingOptions.IdTokenExpirySeconds,
                tokenIssuingOptions.Issuer,
                audience,
                nonce,
                claims,
                jsonWebKey
                );


            return idToken;
        }

        private string GenerateAccessToken(string userId, string audience, string nonce, JsonWebKey jsonWebKey)
        {
            // access_token can be the same as id_token, but here we might have different values for expirySeconds so we use 2 different functions

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId)
            };

            var idToken = JwtGenerator.GenerateJWTToken(
                tokenIssuingOptions.AccessTokenExpirySeconds,
                tokenIssuingOptions.Issuer,
                audience,
                nonce,
                claims,
                jsonWebKey
                );

            return idToken;
        }

        private static string GenerateAuthenticationCode()
        {
            return Guid.NewGuid().ToString("N");
        }

        public IActionResult Cancel(AuthenticationRequestModel authenticateRequest)
        {
            return View();
        }

        private static void ValidateAuthenticateRequestModel(AuthenticationRequestModel authenticateRequest)
        {
            ArgumentNullException.ThrowIfNull(authenticateRequest, nameof(authenticateRequest));

            if (string.IsNullOrEmpty(authenticateRequest.ClientId))
            {
                throw new Exception("client_id required");
            }

            if (string.IsNullOrEmpty(authenticateRequest.ResponseType))
            {
                throw new Exception("response_type required");
            }

            if (string.IsNullOrEmpty(authenticateRequest.Scope))
            {
                throw new Exception("scope required");
            }

            if (string.IsNullOrEmpty(authenticateRequest.RedirectUri))
            {
                throw new Exception("redirect_uri required");
            }
        }

        private static CodeFlowResponseModel BuildCodeFlowResponseModel(AuthenticationRequestModel authenticateRequest, string code)
        {
            return new CodeFlowResponseModel() { 
                Code = code,
                State = authenticateRequest.State
            };
        }
    }
}
