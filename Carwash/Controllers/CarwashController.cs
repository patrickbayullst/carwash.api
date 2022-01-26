using Carwash.Services;
using Carwash.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Carwash.Controllers
{
    [Route("api/v1/[controller]")]
    public class CarwashController : ControllerBase
    {
        private readonly CarwashService _carwashService;
        private readonly TokenUtility _tokenUtility;

        public CarwashController(CarwashService carwashService, TokenUtility tokenUtility)
        {
            _carwashService = carwashService;
            _tokenUtility = tokenUtility;
        }

        [HttpPost("{carwashId}/start")]
        public async Task<IActionResult> StartCarwash(string carwashId)
        {
            if (string.IsNullOrWhiteSpace(carwashId))
                return BadRequest();

            var jwtToken = _tokenUtility.GetFromHttpContext(HttpContext);

            await _carwashService.StartCarwash(jwtToken.UserId, jwtToken.IsSubscribed, carwashId);

            return Ok();
        }

        [HttpGet("{carwashId}")]
        public async Task<IActionResult> GetCarwash(string carwashId)
        {
            if (string.IsNullOrWhiteSpace(carwashId))
                return BadRequest();



            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCarwashes()
        {
            return Ok();
        }
    }
}
