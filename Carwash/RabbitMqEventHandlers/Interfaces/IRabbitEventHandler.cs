namespace Carwash.RabbitMqEventHandlers.Interfaces
{
    public interface IRabbitEventHandler
    {
        Task<bool> HandleEventAsync(string body);
    }
}
