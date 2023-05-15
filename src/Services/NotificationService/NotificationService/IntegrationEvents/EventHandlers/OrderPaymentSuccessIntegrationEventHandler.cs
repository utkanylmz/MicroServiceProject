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
    public class OrderPaymentSuccessIntegrationEventHandler:IIntegrationEventHandler<OrderPaymentSuccessIntegrationEvent>
    {
        private readonly ILogger<OrderPaymentSuccessIntegrationEventHandler> _logger;

        public OrderPaymentSuccessIntegrationEventHandler(ILogger<OrderPaymentSuccessIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handler(OrderPaymentSuccessIntegrationEvent @event)
        {
            //Send Fail Notification (Sms,Email,Push)
            _logger.LogInformation($"Order Payment success with OrderId : {@event.OrderId} ");
            return Task.CompletedTask;
        }
    }
}
