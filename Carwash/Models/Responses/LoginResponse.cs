using System.Net;

namespace Carwash.Models.Responses
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public bool DarkTheme { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public bool Admin { get; set; }
        public bool IsSubscribed { get; set; }

        public (HttpStatusCode StatusCode, string ErrorMessage) Success { get; set; }
    }
}
