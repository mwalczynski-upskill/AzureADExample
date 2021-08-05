using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AzureADExample.FirstApi.Controllers
{
    [ApiController]
    [Route("/ping")]
    public class PingController : ControllerBase
    {

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok("Ok from FirstApi");
        }
    }
}
