using Carwash.Models.Requests;
using Carwash.Repositories;
using Carwash.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carwash.Controllers
{
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            return Ok();
        }

        [HttpGet]   
        public async Task<IActionResult> GetTest()
        {
            var userRepositry = new UserRepository();
            //await userRepositry.CreateUser();
            await userRepositry.GetAsync("patrickbay");
            return Ok();
        }
    }
}
