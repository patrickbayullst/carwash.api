using MongoDB.Driver;

namespace Carwash.Repositories
{
    public class BaseRepository
    {
        public IMongoDatabase GetDatabase()
        {
            //var client = new MongoClient("mongodb+srv://admin:7b1JOcdLqsQSFyFD@cluster0.lel3k.mongodb.net/carwash?retryWrites=true&w=majority");
            var client = new MongoClient("mongodb://localhost:27017");
            return client.GetDatabase("carwash");
        }
    }
}
