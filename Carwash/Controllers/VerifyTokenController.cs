using Microsoft.AspNetCore.Mvc;

namespace Carwash.Controllers
{
    [Route("api/v1/token")]
    public class VerifyTokenController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> CheckToken()
        {
            return Ok();
        }
    }
}
