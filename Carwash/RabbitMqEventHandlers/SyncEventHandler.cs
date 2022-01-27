using Carwash.Enumerations;
using Carwash.Models.Settings;
using Carwash.RabbitMqEventHandlers.Interfaces;
using Carwash.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Carwash.RabbitMqEventHandlers
{
    public class SyncEventHandler : IRabbitEventHandler
    {
        private readonly CarwashRepository _carwashRepository;
        private static int Seconds = 30;
        public SyncEventHandler(CarwashRepository carwashRepository)
        {
            _carwashRepository = carwashRepository;
        }

        public async Task<bool> HandleEventAsync(string eventBody)
        {
            var body = JsonConvert.DeserializeObject<RabbitEvent>(eventBody);

            await _carwashRepository.UpdateStatus(body.CarwashId, StatusEnum.Running);
            PublishRabbitEvent(body.CarwashId, StatusEnum.Running);

            await Task.Delay(Seconds * 100);

            var carWash = await _carwashRepository.GetCarwashById(body.CarwashId);

            if (carWash.Status == StatusEnum.Running)
            {
                await _carwashRepository.UpdateStatus(body.CarwashId, StatusEnum.Available);
                PublishRabbitEvent(body.CarwashId, StatusEnum.Available);
            }

            return true;
        }

        public void PublishRabbitEvent(string carwashId,StatusEnum status)
        {
            var date = DateTimeOffset.Now.AddSeconds(Seconds).ToUnixTimeMilliseconds();

            if (status != StatusEnum.Running)
                date = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var body = new
            {
                CarwashId = carwashId,
                Status = status,
                EndDate = date 
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

            channel.BasicPublish("amq.topic", "testqueue", props, bodyAsBytes);
        }
    }

    public class RabbitEvent
    {
        public string CarwashId { get; set; }

        public StatusEnum Status { get; set; }
    }
}
