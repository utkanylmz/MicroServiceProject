using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using PaymentService.Api.IntegrationEvents.Events;

namespace PaymentService.Api.IntegrationEvents.EventHandler
{
    public class OrderStartedIntegrationEventHandler:IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        private readonly IConfiguration _configuration;
        private readonly IEventBus _eventBus;
        private readonly ILogger<OrderStartedIntegrationEventHandler> _logger;

        public OrderStartedIntegrationEventHandler(IConfiguration configuration,
            IEventBus eventBus, ILogger<OrderStartedIntegrationEventHandler> logger)
        {
            _configuration = configuration;
            _eventBus = eventBus;
            _logger = logger;
        }
        //Order service eventbusa bir OrderStarted eventi gönderdiğinde onu dinleyen payment servicesinin
        //ilgili Handler metodu devreye girecek ve ödeme işslemini yüretecek eğer ödeme işlemi başarılı olursa
        // OrderService EventBus'a ben x ıdli orderın ödemesini başarılı bir şekilde aldım ve ya x idli order'ın ödemesini
        //alamadım diye haber verecek. Ve ilgili mesajı dinleyen servicesler ona göre işlemlerine devam edecek. 
        public Task Handler(OrderStartedIntegrationEvent @event)
        {
            //Normalde burada bir banka sistemine bağlanıp ödeme işlemleri yapılır.
            //configuration True dönecek fake ödeme sistemi oluştuyoruz 
            string keyword = "PaymentSuccess";
            bool paymentSuccessFlag = _configuration.GetValue<bool>(keyword);

            IntegrationEvent paymentEvent = paymentSuccessFlag
                  ? new OrderPaymentSuccessIntegrationEvent(@event.OrderId)
                  : new OrderPaymentFailedIntegrationEvent(@event.OrderId, "This is a fake error message");
            //Payment işlemi başarılı ise OrderPaymentSuccessIntegrationEvent'i değilse OrderPaymentFailedIntegrationEvent'i üret
            //Loggunu tutma işlemi
            _logger.LogInformation($"OrderCreatedIntegrationEventHandler in PaymentService is fired with PaymentSuccess: {paymentSuccessFlag}, orderId: {@event.OrderId}");
            //Eventi Publish ediyoruz (eventbus'a gönderiyoruz).
            _eventBus.Publish(paymentEvent);
            return Task.CompletedTask;
        }
    }
}
