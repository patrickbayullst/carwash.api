using Carwash.Enumerations;
using Carwash.RabbitMqEventHandlers.Interfaces;
using Carwash.Repositories;
using Newtonsoft.Json;

namespace Carwash.RabbitMqEventHandlers
{
    public class SyncEventHandler : BaseHandler, IRabbitEventHandler
    {
        private readonly CarwashRepository _carwashRepository;
        private static int Seconds = 15;
        public SyncEventHandler(CarwashRepository carwashRepository)
        {
            _carwashRepository = carwashRepository;
        }

        public async Task<bool> HandleEventAsync(string eventBody)
        {
            var body = JsonConvert.DeserializeObject<RabbitEvent>(eventBody);
            var startDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var endDate = DateTimeOffset.Now.AddSeconds(Seconds).ToUnixTimeMilliseconds();

            await _carwashRepository.UpdateStatus(body.CarwashId, StatusEnum.Running, endDate, startDate);
            PublishRabbitEvent(body.Name,body.CarwashId, "testqueue", StatusEnum.Running, endDate, startDate);
            PublishRabbitEvent(body.Name, body.CarwashId, "admin", StatusEnum.Running, endDate, startDate);
            await Task.Delay(Seconds * 1000);

            var carWash = await _carwashRepository.GetCarwashById(body.CarwashId);

            if (carWash.Status == StatusEnum.Running)
            {
                await _carwashRepository.UpdateStatus(body.CarwashId, StatusEnum.Available, 0, 0);
                PublishRabbitEvent(body.Name, body.CarwashId, "testqueue", StatusEnum.Available, 0, 0);
                PublishRabbitEvent(body.Name, body.CarwashId, "admin", StatusEnum.Available, endDate, startDate);
            }

            return true;
        }
    }

    public class RabbitEvent
    {
        public string Name { get; set; }

        public string CarwashId { get; set; }

        public StatusEnum Status { get; set; }
    }
}
