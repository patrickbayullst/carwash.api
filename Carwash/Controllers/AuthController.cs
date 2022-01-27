using Carwash.Models.Requests;
using Carwash.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _authService.Register(model);

            if (result == null)
                return Unauthorized();

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody]Token token)
        {

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var loginResult = await _authService.Login(model);

            if(loginResult.Success.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return StatusCode((int)loginResult.Success.StatusCode, loginResult.Success.ErrorMessage);
            }

            return Ok(new
            {
                loginResult.Lastname,
                loginResult.AccessToken,
                loginResult.Firstname,
                loginResult.DarkTheme,
                loginResult.Email,
                loginResult.Admin
            });
        }

        public record Token
        {
            public Guid RefreshToken { get; set; }
        }
    }
}