using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using PaymentService.Api.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.IntegrationEvents.EventHandlers
{
    public class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
    {
        private readonly ILogger<OrderPaymentFailedIntegrationEventHandler> _logger;

        public OrderPaymentFailedIntegrationEventHandler(ILogger<OrderPaymentFailedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handler(OrderPaymentFailedIntegrationEvent @event)
        {
            //Send Fail Notification (Sms,Email,Push)
            _logger.LogInformation($"Order Payment failed with OrderId : {@event.OrderId} ,ErrorMessage : {@event.ErrorMessage}");
            return Task.CompletedTask;
        }
    }
}
