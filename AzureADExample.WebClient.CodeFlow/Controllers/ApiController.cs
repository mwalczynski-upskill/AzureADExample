using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AzureADExample.WebClient.CodeFlow.Controllers
{
    public class ApiController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ApiController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [Authorize]
        [Route("call-first")]
        public async Task<IActionResult> GetFirst()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var httpClient = this.httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44394/ping");
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

            var apiResponse = await httpClient.SendAsync(request);

            var response = new
            {
                Path = "CodeFlow -> FirstApi",
                ApiResponse = apiResponse
            };

            return Ok(response);
        }

        [Authorize]
        [Route("call-second")]
        public async Task<IActionResult> GetSecond()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var httpClient = this.httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44333/ping");
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

            var apiResponse = await httpClient.SendAsync(request);

            var response = new
            {
                Path = "CodeFlow -> SecondApi",
                ApiResponse = apiResponse
            };

            return Ok(response);
        }
    }
}
