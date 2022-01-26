using MongoDB.Driver;

namespace Carwash.Repositories
{
    public class BaseRepository
    {
        public IMongoDatabase GetDatabase()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            return client.GetDatabase("carwash");
        }
    }
}
