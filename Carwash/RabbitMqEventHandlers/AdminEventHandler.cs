using Carwash.RabbitMqEventHandlers.Interfaces;
using Carwash.Repositories;
using Newtonsoft.Json;

namespace Carwash.RabbitMqEventHandlers
{
    public class AdminEventHandler : BaseHandler, IRabbitEventHandler
    {
        private readonly CarwashRepository _carwashRepository;

        public AdminEventHandler(CarwashRepository carwashRepository)
        {
            _carwashRepository = carwashRepository;
        }

        public async Task<bool> HandleEventAsync(string eventBody)
        {
            var body = JsonConvert.DeserializeObject<RabbitEvent>(eventBody);

            if (body.Status != Enumerations.StatusEnum.Running)
            {
                await _carwashRepository.UpdateStatus(body.CarwashId, body.Status, 0, 0);

                PublishRabbitEvent(body.Name, body.CarwashId, "admin", body.Status);
                PublishRabbitEvent(body.Name, body.CarwashId, "testqueue", body.Status, 0, 0);
                return true;
            }

            return true;
        }
    }
}
