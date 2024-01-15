using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OIDCDemo.Client.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace OIDCDemo.Client.Controllers
{
    public class ApiController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        public ApiController(UserManager<IdentityUser> userManager,IHttpClientFactory httpClientFactory) { 
            this.httpClientFactory = httpClientFactory;
        }

        [Route("/api/weatherforcast/v1/free")]
        [HttpGet]
        public async Task<IActionResult> GetFreeWeatherForcastAsync()
        {
            var httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync("https://localhost:7102/weatherforcast/free");
            if (response != null && response.IsSuccessStatusCode)
            {
                return Json(JsonSerializer.Deserialize<WeatherForecast[]>(await response.Content.ReadAsStringAsync()));
            }
            else
            {
                return Content(response?.ReasonPhrase ?? "Error");
            }
        }

        [Route("/api/weatherforcast/v1/detailed")]
        [HttpGet]
        public async Task<IActionResult> GetDetailedWeatherForcastAsync()
        {
            var httpClient = httpClientFactory.CreateClient();
            // send access_token 
            var accessTokenClaim = User.Claims.Where(c => c.Type == "access_token").FirstOrDefault();
            if (accessTokenClaim != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenClaim.Value);
            }

            var response = await httpClient.GetAsync("https://localhost:7102/weatherforcast/detailed");
            if (response != null && response.IsSuccessStatusCode)
            {
                return Json(JsonSerializer.Deserialize<WeatherForecast[]>(await response.Content.ReadAsStringAsync()));
            }
            else
            {
                return Content(response?.ReasonPhrase ?? "Error");
            }
        }
    }
}
