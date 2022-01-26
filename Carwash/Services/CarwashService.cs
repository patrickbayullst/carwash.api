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
        private readonly RabbitMQConfig _rabbitMQConfig;

        public CarwashService(PaymentHistoryRepository paymentHistory, IOptions<RabbitMQConfig> rabbitMqConfig)
        {
            _paymentHistory = paymentHistory;
            _rabbitMQConfig = rabbitMqConfig.Value;
        }

        public async Task StartCarwash(string userId, bool isSubscribed, string carwashId)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = _rabbitMQConfig.Host,
                UserName = _rabbitMQConfig.User,
                Password = _rabbitMQConfig.Password,
                VirtualHost = _rabbitMQConfig.Vhost,
                DispatchConsumersAsync = true
            };

            var body = new
            {
                CarwashId = carwashId,
                Status = (int)StatusEnum.Running
            };

            var bodyAsString = JsonConvert.SerializeObject(body);
            var bodyAsBytes = Encoding.UTF8.GetBytes(bodyAsString);

            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("amq.topic", "topic", true, false);

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
