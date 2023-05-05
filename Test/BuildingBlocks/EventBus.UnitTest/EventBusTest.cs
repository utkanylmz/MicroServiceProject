using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.EventHandler;
using EventBus.UnitTest.Events.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventBus.UnitTest
{
    public class EventBusTest
    {
        private ServiceCollection services;

        public EventBusTest()
        {
           services = new ServiceCollection();
            services.AddLogging(configure=>configure.AddConsole());
        }


        [Fact]
        public void subscribe_event_on_rabbitmq_test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
              return  EventBusFactory.Create(GetRabbitMqConfig(),sp);
            });
            var sp = services.BuildServiceProvider();

            var eventBus=sp.GetRequiredService<IEventBus>();
            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
          //  eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

        }

        [Fact]
        public void subscribe_event_on_azure_test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {

                return EventBusFactory.Create(GetAzureConfig(), sp);
            });
            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();
            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        }


        [Fact]
        public void SendMassageToRabbitMq()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetRabbitMqConfig(), sp);
            });
            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.Publish(new OrderCreatedIntegrationEvent(1));
        }
       
        [Fact]
        public void SendMassageToAzure()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetAzureConfig(), sp);
            });
            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.Publish(new OrderCreatedIntegrationEvent(1));
        }
        private EventBusConfig GetAzureConfig()
        {
           return new EventBusConfig ()
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "MicroServiceProjectTopicName",
                EventBusType = EventBusType.AzureServiceBus,
                EventNameSuffix = "IntegrationEvent",
                EventBusConnectionString = ""
            };
        }

        private EventBusConfig GetRabbitMqConfig()
        {
            return new EventBusConfig()
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "MicroServiceProjectTopicName",
                EventBusType = EventBusType.RabbitMQ,
                EventNameSuffix = "IntegrationEvent"
                //Connection = new ConnectionFactory()
                //{
                //    HostName= "localhost",
                //    Port=5672,
                //    UserName="guest",
                //    Password="guest"
            };
        }
    }
}