using EventBus.Base.Abstraction;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private static void Main(string[] args)
    {
       ServiceCollection services = new ServiceCollection();

    }

    private void ConfigureServices(ServiceCollection services)
    {
       services.AddLogging(configure => configure.AddConsole());
       services.AddTransient<OrderStartedIntegrationEventHandler>();
       services.AddSingleton<IEventBus>(sp =>
        {
            var config = new EventBusConfig
            {
                ConnectionRetryCount = 5,
                EventNameSuffix = "IntegrationEvent",
                SubscriberClientAppName = "PaymentService",
                EventBusType = EventBusType.RabbitMQ
            };

            return EventBusFactory.Create(config, sp);
        });

    }
}