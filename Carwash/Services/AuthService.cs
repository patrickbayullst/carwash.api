using Carwash.Models.Requests;
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

        public async Task Login(LoginRequest loginRequest)
        {

        }

        public async Task<string> Register(RegisterUserRequest model)
        {
            var userExists = await _userRepository.UserExists(model.Username);

            if (userExists)
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
                Username = model.Username
            };

            await _userRepository.CreateUser(fullUserModel);

            return null;
        }
    }
}
