using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AzureADExample.FirstApi.Controllers
{
    [ApiController]
    [Route("/api")]
    public class FirstApiController : ControllerBase
    {
        private readonly IHttpClientFactory httpClientFactory;

        public FirstApiController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Authorize]
        [Route("call-second")]
        public async Task<IActionResult> GetSecond()
        {
            var httpClient = this.httpClientFactory.CreateClient();

            var currentToken = await HttpContext.GetTokenAsync("access_token");
            var token = await httpClient.RequestTokenAsync(
                new TokenRequest
                {
                    Address = "https://login.microsoftonline.com/d33894ee-4acb-44de-8514-40aacd156434/oauth2/v2.0/token",
                    GrantType = "urn:ietf:params:oauth:grant-type:jwt-bearer",
                    ClientId = "6be1b92e-8fdc-4c63-bf95-a2ade01dafa4",
                    ClientSecret = "e8T~.z20fQT4y~fhlM7949~J_RF_MgGorY",
                    Parameters =
                    {
                        { "assertion", currentToken },
                        { "scope", "api://SecondApiApplicationIdUri/FullAccessSecondApi"},
                        { "requested_token_use", "on_behalf_of"}
                    }
                });

            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44333/api");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var apiResponse = await httpClient.SendAsync(request);

            var response = new
            {
                ApiResponse = apiResponse,
                Content = $"Ok from FirstApi. SecondApi returned: {await apiResponse.Content.ReadAsStringAsync()}"
            };

            return Ok(response);
        }
    }
}
