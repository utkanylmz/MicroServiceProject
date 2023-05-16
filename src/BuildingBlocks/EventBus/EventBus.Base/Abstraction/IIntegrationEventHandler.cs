using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstraction
{
    public interface IIntegrationEventHandler<TIntegrationEvent>:  IntegrationEventHandlerBase where TIntegrationEvent:IntegrationEvent
    {
        //IIntegrationEventHandler içerisine dinamik bir TIntegrationEvent tipi alıcak TIntegrationEvent tipi sadece
        //IntegrationEvent tipinde olmak zorunda. IntegrationEvent yarattığımız zaman IIntegrationEventHandler interfacesi
        //kullanmış olucaz ve içerisinde ki handle metoduna bize gelmiş olan IntegrationEvent vereceğiz
        //ve uygulammazı onun içerisinde yapacağız
        Task Handler(TIntegrationEvent @event);
    }

    public interface IntegrationEventHandlerBase
    {
        //MarkUp Interface
    }
}
