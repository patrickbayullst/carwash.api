using Carwash.Models.Users;
using MongoDB.Driver;

namespace Carwash.Repositories
{
    public class UserRepository : BaseRepository
    {
        private readonly IMongoCollection<FullUserModel> _users;
        public UserRepository()
        {
            var database = GetDatabase();
            _users = database.GetCollection<FullUserModel>("users");
        }

        public async Task<FullUserModel> GetUser(string username)
        {
            var userSearch = await _users.FindAsync(a => a.Email == username);
            return await userSearch.SingleOrDefaultAsync();
        }

        public async Task<FullUserModel> GetUserById(string id)
        {
            var userSearch = await _users.FindAsync(a => a.Id == id);

            return await userSearch.SingleOrDefaultAsync();
        }

        public async Task SetSubscription(string userId, bool subscriptionStatus)
        {
            await _users.UpdateOneAsync(Builders<FullUserModel>.Filter.Eq(a => a.Id, userId), Builders<FullUserModel>.Update.Set(a => a.IsSubscribed, subscriptionStatus));
        }

        public async Task SetDarkTheme(string userId, bool darkTheme)
        {
            await _users.UpdateOneAsync(Builders<FullUserModel>.Filter.Eq(a => a.Id, userId), Builders<FullUserModel>.Update.Set(a => a.DarkTheme, darkTheme));
        }

        public async Task CreateUser(FullUserModel fullUser)
        {
            await _users.InsertOneAsync(fullUser);
        }
    }
}
