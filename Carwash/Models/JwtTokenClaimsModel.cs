namespace Carwash.Models
{
    public class JwtTokenClaimsModel
    {
        public string UserId { get; set; }

        public string Email { get; set; }
        public bool IsSubscribed { get; set; }
    }
}
