using Carwash.Models.Users;
using Carwash.Utilities;
using MongoDB.Driver;

namespace Carwash.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<FullUserModel> _users;
        public UserRepository()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var tester = client.GetDatabase("carwash");
            _users = tester.GetCollection<FullUserModel>("users");
        }

        public async Task<bool>UserExists(string username)
        {
            var userSearch = await _users.FindAsync(a => a.Username == username);

            var result = userSearch.SingleOrDefaultAsync();

            if (result != null)
                return true;

            return false;
        }

        public async Task<FullUserModel> GetUser(string username)
        {
            var userSearch = await _users.FindAsync(a => a.Username == username);

            var result = await userSearch.SingleOrDefaultAsync();

            return result;
        }


        public async Task CreateUser(FullUserModel fullUser)
        {
            await _users.InsertOneAsync(fullUser);
        }
    }

}
