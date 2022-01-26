using Microsoft.AspNetCore.Mvc;

namespace Carwash.Controllers
{
    [Route("api/v1/[controller]")]
    public class CarwashController : ControllerBase
    {
        [HttpPost("{carwashId}/start")]
        public async Task<IActionResult> StartCarwash(string carwashId)
        {
            if (string.IsNullOrWhiteSpace(carwashId))
                return BadRequest();

            return Ok();
        }

        [HttpGet("{carwashId}")]
        public async Task<IActionResult> GetCarwash(string carwashId)
        {
            if (string.IsNullOrWhiteSpace(carwashId))
                return BadRequest();


            return Ok();
        }
    }
}
