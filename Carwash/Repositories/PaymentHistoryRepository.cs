using MongoDB.Driver;

namespace Carwash.Repositories
{
    public class PaymentHistoryRepository : BaseRepository
    {
        private IMongoCollection<PaymentHistory> _paymentHistory;

        public PaymentHistoryRepository()
        {
            var database = GetDatabase();
            _paymentHistory = database.GetCollection<PaymentHistory>("paymentHistory");
        }

        public async Task<List<PaymentHistory>> GetPaymentHistoryForUser(string userId)
        {
            var paymentSearch = await _paymentHistory.FindAsync(a => a.UserId == userId);
            return paymentSearch.ToList();
        }

        public async Task InsertPaymentHistory(string userId, bool isSubscribed, string carwashId)
        {
            var paymentHistory = new PaymentHistory
            {
                UserId = userId,
                CarwashId = carwashId,
                Date = DateTime.Now,
                Id = Guid.NewGuid().ToString(),
                Price = isSubscribed ? 0 : 100,
                Subscribed = isSubscribed
            };

            await _paymentHistory.InsertOneAsync(paymentHistory);
        }
    }

    public record PaymentHistory
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string CarwashId { get; set; }

        public bool Subscribed { get; set; }

        public DateTime Date { get; set; }

        public decimal Price { get; set; }
    }
}
