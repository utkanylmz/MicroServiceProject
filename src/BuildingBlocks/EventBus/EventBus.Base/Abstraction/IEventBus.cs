using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstraction
{
    public interface IEventBus
    {
        // IEventBus uygularımızın microserviceslerimizin hangi eventbus'ı subscript edeceğini söylediği söylediği bir event bus olacak
        //Bu eventbustan iki tanede ayrı eventbus çıkıyor.Biri RabbitMq biride AzureServiceBus bu iki eventbusta buraki metotları kullanacak


        void Publish(IntegrationEvent @event);
        //Servicesimiz bir event fırlatacağı zaman bu Publish metodunu kullacak

        void Subscribe<T,TH>() where T:IntegrationEvent where TH : IIntegrationEventHandler<T>;
        // RabbitMq'ya yada AzureServiceBus'a gideceğiz ilgili kanalları oluşturacaz o kanallları dinnlemeye başlayacağız,Consume
        //eden queueları consume eden metotları yazacağız bu subscribe işlemelerini geliştirmek için
        //IntegrationEvent,IntegrationEventHandler parametrelerini alacağız. 
        void UnSubscribe<T,TH>() where T:IntegrationEvent where TH:IIntegrationEventHandler<T>;
    }
}
