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
        private readonly CarwashRepository _carwashRepository;
        public CarwashService(PaymentHistoryRepository paymentHistory, CarwashRepository carwashRepository)
        {
            _paymentHistory = paymentHistory;
            _carwashRepository = carwashRepository;
        }

        public async Task SetAdminStatus(string carwashId, StatusEnum status)
        {
            var carwash = await _carwashRepository.GetCarwashById(carwashId);
            var body = new
            {
                CarwashId = carwashId,
                Status = status,
                Name = carwash.Name
            };

            var bodyAsString = JsonConvert.SerializeObject(body);
            var bodyAsBytes = Encoding.UTF8.GetBytes(bodyAsString);

            PublishMessage("Any", "testqueue", bodyAsBytes);
            await _carwashRepository.UpdateStatus(body.CarwashId, body.Status, 0, 0);
        }

        public async Task StartCarwash(string userId, bool isSubscribed, string carwashId)
        {

            var carwash = await _carwashRepository.GetCarwashById(carwashId);
            var body = new
            {
                CarwashId = carwashId,
                carwash.Name
            };

            var bodyAsString = JsonConvert.SerializeObject(body);
            var bodyAsBytes = Encoding.UTF8.GetBytes(bodyAsString);

            PublishMessage("Sync", "startCarwash", bodyAsBytes);

            await _paymentHistory.InsertPaymentHistory(userId, isSubscribed, carwashId);
        }

        private void PublishMessage(string eventName, string routingKey, byte[] body)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            var headers = new Dictionary<string, object>
            {
                {"EventName", eventName }
            };
            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Headers = headers;

            channel.BasicPublish("amq.topic", routingKey, properties, body);
        }
    }
}
