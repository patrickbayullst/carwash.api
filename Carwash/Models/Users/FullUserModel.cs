namespace Carwash.Models.Users
{
    public class FullUserModel
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public bool IsSubscribed { get; set; }

        public bool Admin { get; set; }

        public bool DarkTheme { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string RefreshToken { get; set; }
    }
}
