using Carwash.Models.Requests;
using Carwash.Repositories;
using Carwash.Services;
using Carwash.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Carwash.Controllers
{
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly TokenUtility _tokenUtility;
        private readonly UserRepository _userRepository;
        private readonly AuthService _authService;

        public UserController(TokenUtility tokenUtility, UserRepository userRepository, AuthService authService)
        {
            _tokenUtility = tokenUtility;
            _authService = authService;
            _userRepository = userRepository;
        }

        [HttpPut("settings/subscription")]
        public async Task<IActionResult> SetSubscription([FromBody] UpdateSubscriptionStatusRequest model)
        {
            var jwtTokenClaims = _tokenUtility.GetFromHttpContext(HttpContext);

            if (jwtTokenClaims == null)
                return Unauthorized();

            await _userRepository.SetSubscription(jwtTokenClaims.UserId, model.SubscriptionStatus);

            return Ok();
        }

        [HttpPut("settings/darktheme")]
        public async Task<IActionResult> SetDarkTheme([FromBody] UpdateDarkTheme model)
        {
            var jwtTokenClaims = _tokenUtility.GetFromHttpContext(HttpContext);

            if (jwtTokenClaims == null)
                return Unauthorized();

            await _userRepository.SetDarkTheme(jwtTokenClaims.UserId, model.EnableDarktheme);

            return Ok();
        }

        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings()
        {
            var jwtTokenClaims = _tokenUtility.GetFromHttpContext(HttpContext);

            var user = await _userRepository.GetUserById(jwtTokenClaims.UserId);

            return Ok(new
            {
                user.DarkTheme,
                user.IsSubscribed
            });
        }

        [HttpPut("settings/update")]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateSettings model)
        {
            var jwtTokenClaims = _tokenUtility.GetFromHttpContext(HttpContext);

            if (jwtTokenClaims == null)
                return Unauthorized();

            await _userRepository.SetDarkTheme(jwtTokenClaims.UserId, model.DarkTheme);
            await _userRepository.SetSubscription(jwtTokenClaims.UserId, model.IsSubscribed);
            var user = await _authService.UserResponse(jwtTokenClaims.UserId);
            return Ok(user);
        }
    }

    public record UpdateDarkTheme
    {
        public bool EnableDarktheme { get; set; }
    }

    public record UpdateSettings
    {
       public bool DarkTheme { get; set; }
       public bool IsSubscribed { get; set; }
    }
}
