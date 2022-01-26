using Carwash.Models.Requests;
using Carwash.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carwash.Controllers
{
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _authService.Register(model);

            if (result == null)
                return Unauthorized();

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var loginResult = await _authService.Login(model);

            if (loginResult == null)
                return Unauthorized();

            return Ok(loginResult);
        }
    }
}