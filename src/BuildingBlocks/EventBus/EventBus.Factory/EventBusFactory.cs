using EventBus.AzureServiceBus;
using EventBus.Base.Abstraction;
using EventBus.Base;
using EventBus.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Factory
{
    public class EventBusFactory
    {
        public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
        {
            //AzureServiceBus veya RabbitMq'yu kullanmak istediğimizde EventBusFactory classına parametreleri göndererek kullanacağız.
            return config.EventBusType switch
            {
                EventBusType.AzureServiceBus => new EventBusServiceBus(config, serviceProvider),
              _ => new EventBusRabbitMQ(config, serviceProvider)
            };
        }
    }
}
