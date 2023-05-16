using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base
{
    public class EventBusConfig
    {
        //EventBus'ın temel konfigürasyonları
        public int ConnectionRetryCount { get; set; } = 5;
        //RabbitMq-AzureServiceBus' a bağlanırken başarısız olursan 5 kere bağlanmayı dene
        public string DefaultTopicName { get; set; } = "UtkanEventBus";

        public string EventBusConnectionString { get; set; } = String.Empty;
        public string SubscriberClientAppName { get; set; } = String.Empty;
        public string EventNamePrefix { get; set; } = String.Empty;
        //Başından ve sonundan trimlenecek 
        public string EventNameSuffix { get; set; } = "IntegrationEvent";
        public EventBusType EventBusType { get; set; } = EventBusType.RabbitMQ;
        //Dışarıdan EventBusType Gönderilmezse default olarak RabbitMQ ya bağlanılacak
        public object? Connection { get; set; }

        public bool DeleteEventPrefix => !String.IsNullOrEmpty(EventNamePrefix);
        public bool DeleteEventSuffix => !String.IsNullOrEmpty(EventNameSuffix);
    }

    public enum EventBusType
    {
        RabbitMQ = 0,
        AzureServiceBus = 1
    }
}

