using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using PaymentService.Api.IntegrationEvents.Events;
using PaymentService.Api.IntegrationEvents.EventsHandler;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(configure => configure.AddConsole());
builder.Services.AddTransient<OrderStartedIntegrationEventHandler>();
/*
    Normal Sartlarda eventhandleri DI ile biryerden cagirmiyoruz ama BaseEventBusa'da sistemde bir eventtype ile serviceden
    eventhandler getirme islemi var getirdigi servicesin handler metodunu calistiracak eventbusa bir event geldigi zaman bir event handler
    create ediyor create ettikten sonra handler metodunu cagirabiliyor bunun icin dependency injection
    ile sisteme inject etmemiz lazim ki handler metodu calissin.
*/
//Sistemden IEventBus interfacesinden bir tureyen bir nesne istendiginde olusturulacak nesnenin configurasyonlari
builder.Services.AddSingleton<IEventBus>(sp =>
{
    var config = new EventBusConfig
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "PaymentService",
        EventBusType = EventBusType.RabbitMQ
    };
    return EventBusFactory.Create(config, sp);
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();
/*
  Sistem icerisinden bir eventbus create etmesini istiyoruz.Ve Create eder etmez yukarida tanimladigim configurasyonlara sahip bir 
  eventbus nesnesi uretiyor.Ve eventbusa diyoruz ki  OrderStartedIntegrationEvent i dinlemeye basla eger eventbus uzerinde bir
  OrderStartedIntegrationEvent tetiklenirse bana OrderStartedIntegrationEventHandler kullanarak haber ver.
 */
IEventBus eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();

app.Run();

