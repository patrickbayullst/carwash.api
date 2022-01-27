using Carwash.Enumerations;
using Carwash.Models.Requests;
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

        public async Task<Carwash> CreateCarwash(CreateCarwashRequest model)
        {
            var carwash = new Carwash
            {
                Address = model.Address,
                Name = model.Name,
                City = model.City,
                Price = model.Price,
                ZipCode = model.ZipCode,
                Status = StatusEnum.Available,
                Id =Guid.NewGuid().ToString()
            };

            await _carwash.InsertOneAsync(carwash);
            return await GetCarwashById(carwash.Id);
        }

        public async Task<Carwash> GetCarwashById(string id)
        {
            var carwashSearch  = await _carwash.FindAsync(a => a.Id == id);
            return await carwashSearch.SingleOrDefaultAsync();
        }

        public async Task<List<Carwash>> GetAllCarwashes(int limit = 20, int offset = 0)
        {
            return await _carwash.Find(Builders<Carwash>.Filter.Empty).Limit(limit).Skip(offset).ToListAsync();
        }

        public async Task UpdateStatus(string carwashId, StatusEnum status)
        {
            await _carwash.UpdateOneAsync(Builders<Carwash>.Filter.Eq(a => a.Id, carwashId), Builders<Carwash>.Update.Set(a => a.Status, status));
        }
    }

    public record Carwash
    {
        public string Id { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        public StatusEnum Status { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}
