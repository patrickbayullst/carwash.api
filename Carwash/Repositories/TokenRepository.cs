using MongoDB.Driver;

namespace Carwash.Repositories
{
    public class TokenRepository : BaseRepository
    {
        private IMongoCollection<Token> _token;
        public TokenRepository()
        {
            var database = GetDatabase();
            _token = database.GetCollection<Token>("token");
        }
        public async Task<Guid> InsertRefreshToken(string userId)
        {
            var refreshToken = Guid.NewGuid();
            var token = new Token
            {
                ExpirationDate = DateTime.Now.AddDays(5),
                UserId = userId,
                Id = Guid.NewGuid().ToString(),
                RefreshToken = refreshToken
            };

            await _token.InsertOneAsync(token);
            return refreshToken;
        }
    }

    public record Token 
    {
        public string Id { get; set; }

        public Guid RefreshToken { get; set; }

        public string UserId { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}

