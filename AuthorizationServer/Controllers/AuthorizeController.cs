using Microsoft.AspNetCore.Mvc;
using OIDCDemo.AuthorizationServer.Models;
using System.CodeDom.Compiler;
using System.Net.Http;

namespace OIDCDemo.AuthorizationServer.Controllers
{
    public class AuthorizeController : Controller
    {
        private readonly ICodeStorage codeStorage;
        private readonly IHttpClientFactory httpClientFactory;

        public AuthorizeController(ICodeStorage codeStorage, IHttpClientFactory httpClientFactory)
        {
            this.codeStorage = codeStorage;
            this.httpClientFactory = httpClientFactory;
        }

        public IActionResult Index(AuthenticationRequestModel authenticateRequest)
        {
            ValidateAuthenticateRequestModel(authenticateRequest);
            return View(authenticateRequest);
        }

        [HttpPost]
        public IActionResult AuthorizeAsync(AuthenticationRequestModel authenticateRequest, string user, string[] scopes)
        {
            ValidateAuthenticateRequestModel(authenticateRequest);

            string code = GenerateCode();
            if (!codeStorage.TryAddCode(code, DateTime.Now.AddSeconds(60 * 5))) // this code will be expired after 5 minutes
            {
                throw new Exception("Error storing code"); 
            }

            var codeFlowModel = BuildCodeFlowResponseModel(authenticateRequest, code);

            string viewName = "SubmitForm"; // we can change to another view if we need to support response_modes other than form_post

            return View(viewName, new CodeFlowResponseViewModel()
            {
                Code = codeFlowModel.Code,
                RedirectUri = authenticateRequest.RedirectUri,
                State = codeFlowModel.State,
            });
        }

        [HttpPost("/token")]
        public IActionResult GetTokenAsync(string grant_type, string code, string redirect_uri)
        {
            if (grant_type != "authorization_code")
            {
                return StatusCode(404);
            }

            if (!codeStorage.TryGetToken(code, out var _))
            {
                return StatusCode(404);
            }

            // TODO: we need to check redirect_uri == authenticateRequest.redirect_uri

            var result = new AuthenticationResponseModel() { 
                AccessToken = GenerateAccessToken(),
                IdToken = GenerateIdToken(),
                TokenType = "Bearer",
                RefreshToken = GenerateRefreshToken(),
                ExpiresIn = 1200 // valid in 20 minutes
            };

            return Json(result);
        }

        private string GenerateRefreshToken()
        {
            return string.Empty;
        }

        private string GenerateIdToken()
        {
            return string.Empty;
        }

        private string GenerateAccessToken()
        {
            return string.Empty;
        }

        private static string GenerateCode()
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
