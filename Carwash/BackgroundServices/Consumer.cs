using Carwash.Models.Settings;
using Carwash.RabbitMqEventHandlers.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace Carwash.BackgroundServices
{
    public class Consumer : BackgroundService
    {
        private readonly RabbitMQConfig _rabbitConfig;
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceProvider _services;
        public Consumer(IOptions<RabbitMQConfig> rabbitConfig, IServiceProvider services)
        {
            _rabbitConfig = rabbitConfig.Value;
            _services = services;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitConfig.Host,
                UserName = _rabbitConfig.User,
                Password = _rabbitConfig.Password,
                VirtualHost = _rabbitConfig.Vhost,
                DispatchConsumersAsync = true
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            _channel.CallbackException += (chann, args) =>
             {
                 Console.WriteLine(args.Exception);
             };

            consumer.Received += HandleEvent;

            _channel.BasicConsume("testqueue", false, consumer);

            await Task.CompletedTask.ConfigureAwait(false);
        }

        private async Task HandleEvent(object sender, BasicDeliverEventArgs basicDeliverEvent)
        {
            bool eventHandled = await ProcessEvent(sender, basicDeliverEvent);

            try
            {
                if (eventHandled)
                    _channel.BasicAck(basicDeliverEvent.DeliveryTag, false);
                else
                    _channel.BasicNack(basicDeliverEvent.DeliveryTag, false, false);
            }
            catch(AlreadyClosedException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task<bool> ProcessEvent(object sender, BasicDeliverEventArgs basicDeliverEvent)
        {
            var eventName = GetHeaderValue(basicDeliverEvent, "EventName");

            var eventBodyAsArray = basicDeliverEvent.Body.ToArray();
            var eventBodyAsString = Encoding.UTF8.GetString(eventBodyAsArray);

            using var scope = _services.CreateScope();
            return await HandleEventAsync(basicDeliverEvent, eventName, eventBodyAsString, scope);
        }

        private async Task<bool> HandleEventAsync(BasicDeliverEventArgs basicDeliverEvent, string eventName, string eventBody, IServiceScope scope)
        {
            var eventHandlerName = $"Carwash.RabbitMqEventHandlers.{eventName}EventHandler";

            var eventHandlerType = Type.GetType(eventHandlerName);
            var eventHandler = (IRabbitEventHandler)scope.ServiceProvider.GetRequiredService(eventHandlerType);

            return await eventHandler.HandleEventAsync(eventBody);
        }

        private static string GetHeaderValue(BasicDeliverEventArgs basicDeliverEvent, string headerName)
        {
            var value = "Unknown";
            if(basicDeliverEvent.BasicProperties.Headers.TryGetValue(headerName, out object valueObject))
            {
                var bytes = valueObject as byte[];
                value = Encoding.UTF8.GetString(bytes);
            }

            return value;
        }
    }
}
