using Carwash.Models;
using Carwash.Models.Settings;
using Carwash.Models.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Carwash.Utilities
{
    public class TokenUtility
    {
        private readonly AppSettings _appSettings;

        public TokenUtility(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string GenerateToken(FullUserModel model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Token));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("userId", model.Id),
                    new Claim("email", model.Email),
                    new Claim("isSubscribed", model.IsSubscribed.ToString()),
                    new Claim("isAdmin", model.Admin.ToString())
                }),

                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _appSettings.Issuer,
                Audience = _appSettings.Audience,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public JwtTokenClaimsModel GetFromHttpContext(HttpContext context)
        {
            try
            {
                return new JwtTokenClaimsModel
                {
                    UserId = context.User.Claims.Single(a => a.Type == "userId").Value,
                    IsSubscribed = bool.Parse(context.User.Claims.Single(a => a.Type == "isSubscribed").Value),
                    Admin = bool.Parse(context.User.Claims.Single(a => a.Type == "isAdmin").Value)
                };
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
