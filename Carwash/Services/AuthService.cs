using Carwash.Models.Requests;
using Carwash.Models.Responses;
using Carwash.Models.Users;
using Carwash.Repositories;
using Carwash.Utilities;
using System.Net;

namespace Carwash.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;
        private readonly PasswordUtility _passwordUtility;
        private readonly TokenUtility _tokenUtility;
        private readonly TokenRepository _tokenRepository;

        public AuthService(UserRepository userRepository, PasswordUtility passwordUtility, TokenUtility tokenUtility, TokenRepository tokenRepository)
        {
            _tokenUtility = tokenUtility;
            _userRepository = userRepository;
            _passwordUtility = passwordUtility;
            _tokenRepository = tokenRepository;
        }

        public async Task<LoginResponse> Login(LoginRequest model)
        {
            var user = await _userRepository.GetUser(model.Email);

            if (user == null)
                return new LoginResponse
                {
                    Success = (HttpStatusCode.Unauthorized, "UserNotFound")
                };

            var verifyPwd = _passwordUtility.VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt);

            if (!verifyPwd)
                return new LoginResponse
                {
                    Success = (HttpStatusCode.Unauthorized, "BadCredentials")
                };

            var token = _tokenUtility.GenerateToken(user);
            await _tokenRepository.InsertRefreshToken(user.Id);

            return new LoginResponse
            {
                AccessToken = token,
                DarkTheme = user.DarkTheme,
                Email = model.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Admin = user.Admin,
                IsSubscribed = user.IsSubscribed,
                Success = (HttpStatusCode.OK, "OK")
            };
        }

        public async Task<UserResponseModel> UserResponse(string id)
        {
            var user = await _userRepository.GetUserById(id);

            var token = _tokenUtility.GenerateToken(user);

            return new UserResponseModel
            {
                AccessToken = token,
                DarkTheme = user.DarkTheme,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Admin = user.Admin,
                IsSubscribed = user.IsSubscribed

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
                DarkTheme = false,
                Firstname = model.Firstname,
                Lastname =  model.Lastname,
            };

            await _userRepository.CreateUser(fullUserModel);

            var token = _tokenUtility.GenerateToken(fullUserModel);

            return new LoginResponse
            {
                AccessToken = token,
                Email = fullUserModel.Email,
                DarkTheme = fullUserModel.DarkTheme,
                Firstname = fullUserModel.Firstname,
                Lastname = fullUserModel.Lastname,
                Admin = fullUserModel.Admin,
                IsSubscribed = fullUserModel.IsSubscribed
            };
        }

        public record UserResponseModel
        {
            public string AccessToken { get; set; }
            public bool DarkTheme { get; set; }
            public string Email { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public bool Admin { get; set; }
            public bool IsSubscribed { get; set; }
        }
    }
}
