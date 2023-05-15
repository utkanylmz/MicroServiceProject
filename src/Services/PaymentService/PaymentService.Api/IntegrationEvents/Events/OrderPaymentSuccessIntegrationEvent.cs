using EventBus.Base.Events;

namespace PaymentService.Api.IntegrationEvents.Events
{
    public class OrderPaymentSuccessIntegrationEvent:IntegrationEvent
    {
        public OrderPaymentSuccessIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get;  }
    }
}
