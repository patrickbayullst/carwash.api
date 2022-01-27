using Carwash.Enumerations;
using Carwash.Models.Settings;
using Carwash.Repositories;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Carwash.Services
{
    public class CarwashService
    {
        private readonly PaymentHistoryRepository _paymentHistory;

        public CarwashService(PaymentHistoryRepository paymentHistory)
        {
            _paymentHistory = paymentHistory;
        }

        public async Task StartCarwash(string userId, bool isSubscribed, string carwashId)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            var body = new
            {
                CarwashId = carwashId
            };

            var bodyAsString = JsonConvert.SerializeObject(body);
            var bodyAsBytes = Encoding.UTF8.GetBytes(bodyAsString);

            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            var headers = new Dictionary<string, object>
            {
                {"EventName", "Sync" }
            };
            IBasicProperties props = channel.CreateBasicProperties();
            props.Headers = headers;

            channel.BasicPublish("amq.topic", "startCarwash", props, bodyAsBytes);

            await _paymentHistory.InsertPaymentHistory(userId, isSubscribed, carwashId);
        }
    }
}
