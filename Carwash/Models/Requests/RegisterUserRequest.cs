using System.ComponentModel.DataAnnotations;

namespace Carwash.Models.Requests
{
    public class RegisterUserRequest
    {
        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password and confirm password does not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
