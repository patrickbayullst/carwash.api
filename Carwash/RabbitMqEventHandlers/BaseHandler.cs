using Carwash.Enumerations;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Carwash.RabbitMqEventHandlers
{
    public class BaseHandler
    {
        public void PublishRabbitEvent(string name, string carwashId, string routingKey, StatusEnum status, long endDate = 0, long startDate = 0)
        {
            if (status != StatusEnum.Running)
            {
                endDate = 0;
                startDate = 0;
            }

            var body = new
            {
                CarwashId = carwashId,
                Status = status,
                EndDate = endDate,
                StartDate = startDate,
                Name = name
            };

            var bodyAsString = JsonConvert.SerializeObject(body);
            var bodyAsBytes = Encoding.UTF8.GetBytes(bodyAsString);
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            var headers = new Dictionary<string, object>
            {
                {"EventName", "Data" }
            };
            IBasicProperties props = channel.CreateBasicProperties();
            props.Headers = headers;

            channel.BasicPublish("amq.topic", routingKey, props, bodyAsBytes);
        }
    }
}
