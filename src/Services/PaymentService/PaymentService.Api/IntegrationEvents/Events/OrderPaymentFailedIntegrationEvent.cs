using EventBus.Base.Events;

namespace PaymentService.Api.IntegrationEvents.Events
{
    public class OrderPaymentFailedIntegrationEvent:IntegrationEvent
    {

        public int OrderId { get; }
        public string ErrorMessage { get; }
        public OrderPaymentFailedIntegrationEvent(int orderId, string errormessage)
        {
            OrderId = orderId;
            ErrorMessage = errormessage;
        }


    }
}
