using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstraction
{
    public interface IEventBusSubscriptionManager
    {
        
        public  bool IsEmpty { get; }
        //Herhangi bir eventi dinliyormuyuz?
        event EventHandler<string> OnEventRemoved;
        void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        //Subscription ekleme işlemi
        void RemoveSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        //Subscription silme işlemi

        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        //Dışarıdan bir event geldiğinde zaten bu event'i subscribe olup dinleyip dinlemediğimiz konrol eder
        bool HasSubscriptionsForEvent(string eventName);
        //Dışarıdan Eventin adi gönderildiğinde zaten bu event'i subscribe olup dinleyip dinlemediğimiz konrol eder
        Type GetEventTypeByName(string eventName);
        //event name gönderildiğinde onun typını geri göndericez
        void Clear();
        //bütün subscriptionları clear edebilecez
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;
        //Dışarıdan gönderilen bir eventin bütün subscriptionlarını handlerlarını geri döneceğiz
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        //Dışarıdan verilen eventName'in  bütün subscriptionlarını handlerlarını geri döneceğiz
        string GetEventKey<T>();
        //Eventlerin  Rounting key geri dönücek.
    }
}
