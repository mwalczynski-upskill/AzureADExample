using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AzureADExample.WebClient.ImplicitFlow.Controllers
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
            
            var httpClient = this.httpClientFactory.CreateClient();

            var currentToken = await HttpContext.GetTokenAsync("id_token");
            var token = await httpClient.RequestTokenAsync(
                new TokenRequest
                {
                    Address = "https://login.microsoftonline.com/d33894ee-4acb-44de-8514-40aacd156434/oauth2/v2.0/token",
                    GrantType = "urn:ietf:params:oauth:grant-type:jwt-bearer",
                    ClientId = "131f54ba-b3fb-4b71-9465-2288944c857e",
                    ClientSecret = "19TQ4S-LcJ_S.KC9zFyb5-4ENhUy~9~bpJ",
                    Parameters =
                    {
                        { "assertion", currentToken },
                        { "scope", "api://FirstApiApplicationIdUri/FullAccessFirstApi"},
                        { "requested_token_use", "on_behalf_of"}
                    }
                });


            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44394/ping");
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token.AccessToken);

            var apiResponse = await httpClient.SendAsync(request);

            var response = new
            {
                Path = "ImplicitFlow -> FirstApi",
                ApiResponse = apiResponse,
                Content = await apiResponse.Content.ReadAsStringAsync()
            };

            return Ok(response);
        }

        [Authorize]
        [Route("call-second")]
        public async Task<IActionResult> GetSecond()
        {
            var httpClient = this.httpClientFactory.CreateClient();

            var currentToken = await HttpContext.GetTokenAsync("id_token");
            var token = await httpClient.RequestTokenAsync(
                new TokenRequest
                {
                    Address = "https://login.microsoftonline.com/d33894ee-4acb-44de-8514-40aacd156434/oauth2/v2.0/token",
                    GrantType = "urn:ietf:params:oauth:grant-type:jwt-bearer",
                    ClientId = "131f54ba-b3fb-4b71-9465-2288944c857e",
                    ClientSecret = "19TQ4S-LcJ_S.KC9zFyb5-4ENhUy~9~bpJ",
                    Parameters =
                    {
                        { "assertion", currentToken },
                        { "scope", "api://SecondApiApplicationIdUri/FullAccessSecondApi"},
                        { "requested_token_use", "on_behalf_of"}
                    }
                });

            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44333/ping");
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token.AccessToken);

            var apiResponse = await httpClient.SendAsync(request);

            var response = new
            {
                Path = "ImplicitFlow -> SecondApi",
                ApiResponse = apiResponse,
                Content = await apiResponse.Content.ReadAsStringAsync()
            };

            return Ok(response);
        }

        [Authorize]
        [Route("call-first-second")]
        public async Task<IActionResult> GetFirstSecond()
        {
            var httpClient = this.httpClientFactory.CreateClient();

            var currentToken = await HttpContext.GetTokenAsync("id_token");
            var token = await httpClient.RequestTokenAsync(
                new TokenRequest
                {
                    Address = "https://login.microsoftonline.com/d33894ee-4acb-44de-8514-40aacd156434/oauth2/v2.0/token",
                    GrantType = "urn:ietf:params:oauth:grant-type:jwt-bearer",
                    ClientId = "131f54ba-b3fb-4b71-9465-2288944c857e",
                    ClientSecret = "19TQ4S-LcJ_S.KC9zFyb5-4ENhUy~9~bpJ",
                    Parameters =
                    {
                        { "assertion", currentToken },
                        { "scope", "api://FirstApiApplicationIdUri/FullAccessFirstApi"},
                        { "requested_token_use", "on_behalf_of"}
                    }
                });


            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44394/api/call-second");
            request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token.AccessToken);

            var apiResponse = await httpClient.SendAsync(request);
            var json = await apiResponse.Content.ReadAsStringAsync();

            dynamic deserializedResponse = JsonConvert.DeserializeObject(json);

            var response = new
            {
                Path = "ImplicitFlow -> FirstApi -> SecondApi",
                FirstApiResponse = apiResponse,
                SecondApiResponse = deserializedResponse!.ApiResponse,
                Content = deserializedResponse!.Content
            };

            return Ok(response);
        }
    }
}
