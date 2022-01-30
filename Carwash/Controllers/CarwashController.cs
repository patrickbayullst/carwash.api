using Carwash.Enumerations;
using Carwash.Models.Requests;
using Carwash.Repositories;
using Carwash.Services;
using Carwash.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carwash.Controllers
{
    [Route("api/v1/[controller]")]
    public class CarwashController : ControllerBase
    {
        private readonly CarwashService _carwashService;
        private readonly TokenUtility _tokenUtility;
        private readonly CarwashRepository _carwashRepository;

        public CarwashController(CarwashService carwashService, TokenUtility tokenUtility, CarwashRepository carwashRepository)
        {
            _carwashService = carwashService;
            _tokenUtility = tokenUtility;
            _carwashRepository = carwashRepository;
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> CreateCarwash([FromBody] CreateCarwashRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _carwashRepository.CreateCarwash(model);
            return Ok(result);
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

        [HttpPost("{carwashId}/admin/status/{status}")]
        public async Task<IActionResult> AdminSetCarwash([FromRoute] string carwashId, [FromRoute] StatusEnum status)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var jwtToken = _tokenUtility.GetFromHttpContext(HttpContext);

            if (!jwtToken.Admin)
                return Unauthorized();

            await _carwashService.SetAdminStatus(carwashId, status);
            return Ok();
        }

        [HttpGet("admin/list")]
        public async Task<IActionResult> GetAdminCarwashList(int limit = 20, int offset = 0)
        {
            var jwtToken = _tokenUtility.GetFromHttpContext(HttpContext);

            if (!jwtToken.Admin)
                return Unauthorized();

            var carwashes = await _carwashRepository.GetAdminCarwashes(limit, offset);
            return Ok(carwashes);
        }

        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCarwashes(int limit = 20, int offset = 0)
        {

            var carWashes = await _carwashRepository.GetAllCarwashes(limit, offset);

            return Ok(carWashes);
        }
    }

    public record SetCarwashStatus
    {
        public StatusEnum WashStatus { get; set; }
    }
}
