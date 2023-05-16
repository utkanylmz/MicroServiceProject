using EventBus.Base.Abstraction;
using EventBus.Base.SubManagers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Events
{
    public abstract class BaseEventBus:IEventBus
    {
        public readonly IServiceProvider ServiceProvider;
        public readonly IEventBusSubscriptionManager SubsManager;

        public EventBusConfig EventBusConfig { get; set; }

        public BaseEventBus(EventBusConfig config, IServiceProvider serviceProvider)
        {
            EventBusConfig = config;
            ServiceProvider = serviceProvider;
            SubsManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
        }
        public virtual string ProcessEventName(string eventName)
        {
            if (EventBusConfig.DeleteEventPrefix)
                eventName = eventName.TrimStart(EventBusConfig.EventNamePrefix.ToArray());
            if (EventBusConfig.DeleteEventSuffix)
                eventName = eventName.TrimEnd(EventBusConfig.EventNameSuffix.ToArray());
            return eventName;
        }

        public virtual string GetSubName(string eventName)
        {
            return $"{EventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";

        }
        public virtual void Dispose()
        {
            EventBusConfig = null;
            SubsManager.Clear();
        }
        public async Task<bool> ProcessEvent(string eventName, string message)
        {
           //Dışarıdan bir tane eventName ve message gödnerilecek.Bu rabbit mq yada AzureServiceBus tarafından ulaştırılmış bir mesaj.
           //Bu event'in nameini ve mesajını alıyoruz.
            eventName = ProcessEventName(eventName);
            var processed = false;
            if (SubsManager.HasSubscriptionsForEvent(eventName))
            {
                var subscriptions = SubsManager.GetHandlersForEvent(eventName);

                using (var scope = ServiceProvider.CreateScope())
                {
                    foreach (var subscription in subscriptions)
                    {
                        var handler = ServiceProvider.GetService(subscription.HandlerType);
                        if (handler == null) continue;
                        
                        var eventType = SubsManager.GetEventTypeByName($"{EventBusConfig.EventNamePrefix}{eventName}{EventBusConfig.EventNameSuffix}");
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                       
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        await (Task)concreteType.GetMethod("Handler").Invoke(handler, new object[] { integrationEvent });
                    }

                }

                processed = true;
            }
            return processed;
        }




        //Publish Subscribe UnSubscribe işlemlerinin Configürasyonları RabbitMQ'da ve AzureServiceBus'ta farklı uygulnacak diğer metotların
        // implemantasyonları her iki message brokerdada aynı aynı olduğu için onların metotlarının içi dolu 

        public abstract void Publish(IntegrationEvent @event);

        public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;


        public abstract void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}
