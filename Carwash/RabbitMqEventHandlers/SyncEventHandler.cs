using Carwash.Enumerations;
using Carwash.RabbitMqEventHandlers.Interfaces;
using Newtonsoft.Json;

namespace Carwash.RabbitMqEventHandlers
{
    public class SyncEventHandler : IRabbitEventHandler
    {
        public async Task<bool> HandleEventAsync(string eventBody)
        {
            var body = JsonConvert.DeserializeObject<RabbitEvent>(eventBody);

            //Console.WriteLine(body.Hello);

            return true;
        }
    }

    public class RabbitEvent
    {
        public string CarwashId { get; set; }

        public StatusEnum Status { get; set; }
    }
}
