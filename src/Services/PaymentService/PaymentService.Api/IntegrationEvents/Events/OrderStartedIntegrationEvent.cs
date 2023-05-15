using EventBus.Base.Events;

namespace PaymentService.Api.IntegrationEvents.Events
{
    public class OrderStartedIntegrationEvent:IntegrationEvent
    {
        //EventBus'a OrderStartedIntegrationEvent  gönderildiğinde PaymentService harekete geçicek harekete geçmesini sağlayan ise
        //EventHandler Mekanizması olacak 
        public int OrderId { get; set; }
        public OrderStartedIntegrationEvent()
        {
            
        }

        public OrderStartedIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }
    }
}
