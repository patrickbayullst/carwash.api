using Carwash.Models.Requests;
using Carwash.Models.Responses;
using Carwash.Models.Users;
using Carwash.Repositories;
using Carwash.Utilities;

namespace Carwash.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;
        private readonly PasswordUtility _passwordUtility;
        private readonly TokenUtility _tokenUtility;

        public AuthService(UserRepository userRepository, PasswordUtility passwordUtility, TokenUtility tokenUtility)
        {
            _tokenUtility = tokenUtility;
            _userRepository = userRepository;
            _passwordUtility = passwordUtility;
        }

        public async Task<LoginResponse> Login(LoginRequest model)
        {
            var user = await _userRepository.GetUser(model.Email);

            if (user == null)
                return null;

            var verifyPwd = _passwordUtility.VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt);

            if (!verifyPwd)
                return null;

            var token = _tokenUtility.GenerateToken(user);

            return new LoginResponse
            {
                AccessToken = token,
                DarkTheme = user.DarkTheme
            };
        }

        public async Task<LoginResponse> Register(RegisterUserRequest model)
        {
            var user = await _userRepository.GetUser(model.Email);

            if (user != null)
                return null;

            _passwordUtility.CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var fullUserModel = new FullUserModel
            {
                Admin = false,
                Email = model.Email,
                Id = Guid.NewGuid().ToString(),
                IsSubscribed = false,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                DarkTheme = false
            };

            await _userRepository.CreateUser(fullUserModel);

            var token = _tokenUtility.GenerateToken(fullUserModel);

            return new LoginResponse
            {
                AccessToken = token
            };
        }
    }
}
