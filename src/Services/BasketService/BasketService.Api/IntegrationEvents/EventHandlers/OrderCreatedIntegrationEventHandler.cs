using BasketService.Api.Core.Application.Repository;
using EventBus.Base.Abstraction;
using BasketService.Api.IntegrationEvents.Events;
namespace BasketService.Api.IntegrationEvents.EventHandlers
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        private readonly IBasketRepository _repository;
        private readonly ILogger<OrderCreatedIntegrationEvent> _logger;
        //OrderCreatedIntegrationEventHandler metodu publish edilen OrderCreateddIntegrationEvent mesajı işleme alındığında tetiklenecek
        // bu metot tetiklendikden sonra sepet temizlenecek bunun amacı sepeti temizleme işlemi OrderCreateddIntegrationEvent mesajının
        //işleme alındığından emin olduktan sonra siliniyor.
        public OrderCreatedIntegrationEventHandler(IBasketRepository repository, ILogger<OrderCreatedIntegrationEvent> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Handler(OrderCreatedIntegrationEvent @event)
        {
            _logger.LogInformation("-------- Handling integration event: {IntegrationEventId} at BasketService.Api - ({@IntegrationEvent})", @event.Id, @event);

            await _repository.DeleteBasketAsync(@event.UserId);
        }
    }
}
