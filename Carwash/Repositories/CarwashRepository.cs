using Carwash.Enumerations;
using MongoDB.Driver;

namespace Carwash.Repositories
{
    public class CarwashRepository : BaseRepository
    {
        private readonly IMongoCollection<Carwash> _carwash;
        public CarwashRepository()
        {
            var database = GetDatabase();
            _carwash = database.GetCollection<Carwash>("carwash");
        }

        public async Task<List<Carwash>> GetAllCarwashes()
        {
            return await _carwash.Find(Builders<Carwash>.Filter.Empty).ToListAsync();
        }

        public async Task UpdateStatus(string carwashId, StatusEnum status)
        {
            await _carwash.UpdateOneAsync(Builders<Carwash>.Filter.Eq(a => a.Id, carwashId), Builders<Carwash>.Update.Set(a => a.Status, status));
        }
    }

    public record Carwash
    {
        public string Id { get; set; }

        public string Location { get; set; }

        public string City { get; set; }

        public StatusEnum Status { get; set; }

    }
}
