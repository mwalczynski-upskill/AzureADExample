using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AzureADExample.SecondApi.Controllers
{
    [ApiController]
    [Route("/ping")]
    public class SecondApiController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok("Ok from SecondApi");
        }
    }
}
