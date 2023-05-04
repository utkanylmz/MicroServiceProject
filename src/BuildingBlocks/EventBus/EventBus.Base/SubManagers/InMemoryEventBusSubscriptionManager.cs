using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.SubManagers
{
    public class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _eventTypes;
        public event EventHandler<string> OnEventRemoved;
        public Func<string, string> eventNameGetter;

        //Func<string, string> eventNameGetter metodu parametre alma sebebi bize gelen Integration eventlerin örneğin
        //order.created.integration.event bu evente karşılık queue name oluşturcaz sistemizde order.created.integration.event adı bu 
        //olan eventin queuename'inin order.created bu şekilde vermek istiyoruz string verilen event adını trimleyerek bize string
        //queue name verecek
        public InMemoryEventBusSubscriptionManager(Func<string, string> eventNameGetter)
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
            this.eventNameGetter = eventNameGetter;
        }

        //Key var mı yokmu 
        public bool IsEmpty => !_handlers.Keys.Any();

        // sil
        public void Clear() => _handlers.Clear();

        public void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            AddSubscription(typeof(TH), eventName);
            if (_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }

        }
        //Dışarıdan IntegrationEvent,IIntegrationEventHandler handler alıyoruz aldığımız IntegrationEvent'in adını GetEventKey kullanarak 
        //ismini buluyoruz. private AddSubscription metoduna IIntegrationEventHandler'ı ve eventName'i vererek HasSubscriptionForEvent 
        //dictonaryde verilen eventname'de bir key var mı yok mu onu kontrol ediyoruz eğer yoksa (bu event subscribe edilmemişse) _handlers
        //dictonary'ye bu key'i ekliyoruz.Eğer varsa hata fırlatıyoruz
        private void AddSubscription(Type handlerType, string eventName)
        {
            //Bu metottaki temel amaç aynı evente birden fazla kez subscribe olmamak 
            if (!HasSubscriptionForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());
            }
            if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException($"Handler type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }
            _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
        }

        public void RemoveSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var handlerToRemove = FindSubscriptionToRemove<T, TH>();
            var eventName = GetEventKey<T>();
            RemoveHandler(eventName, handlerToRemove);
        }
        
        private void RemoveHandler(string eventName, SubscriptionInfo subsToRemove)
        {
            if (subsToRemove != null)
            {
                _handlers[eventName].Remove(subsToRemove);
                if (!_handlers[eventName].Any())
                {
                    _handlers.Remove(eventName);
                    var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                    if (eventType != null)
                    {
                        _eventTypes.Remove(eventType);
                    }
                    RaiseOnEventRemoved(eventName);
                }
            }
        }

        public string GetEventKey<T>()
        {
            string eventName = typeof(T).Name;
            return eventNameGetter(eventName);
        }

        public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(t => t.Name == eventName);

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];
       
        //Eğer bir eventhandlerdan bir event silindiyse unsubscripte olduysa bu eventi kullananlara haber verir.
        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);
        }
        private SubscriptionInfo FindSubscriptionToRemove<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            return FindSubscriptionToRemove(eventName, typeof(TH));
        }

        private SubscriptionInfo FindSubscriptionToRemove(string eventName, Type handlerType)
        {
            if (!HasSubscriptionForEvent(eventName))
            {
                return null;
            }
            return _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
        }

        public bool HasSubscriptionForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return HasSubscriptionForEvent(key);
        }

        public bool HasSubscriptionForEvent(string eventName) => _handlers.ContainsKey(eventName);

    }
}
