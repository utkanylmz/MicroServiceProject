using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using PaymentService.Api.IntegrationEvents.Events;
using PaymentService.Api.IntegrationEvents.EventHandler;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(configure => configure.AddConsole());
//Normal Sartlarda eventhandleri DI ile biryerden cagirmiyoruz ama BaseEventBusa'da sistemde bir eventtype ile serviceden
//eventhandler getirme islemi var getirdigi servicesin handler metodunu calistiracak eventbusa bir event geldigi zaman bir event handler
//create ediyor create ettikten sonra handler metodunu cagirabiliyor bunun icin dependency injection
//ile sisteme inject etmemiz lazim ki handler metodu calissin.
builder.Services.AddTransient<OrderStartedIntegrationEventHandler>();

//Sistemden IEventBus interfacesinden bir tureyen bir nesne istendiginde olusturulacak nesnenin configurasyonlari

builder.Services.AddSingleton<IEventBus>(sp =>
{
    EventBusConfig config = new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "PaymentService",
        Connection = new ConnectionFactory(),
        EventBusType = EventBusType.RabbitMQ
    };
    return EventBusFactory.Create(config, sp);
});


var app = builder.Build();

// Sistem icerisinden bir eventbus create etmesini istiyoruz.Ve Create eder etmez yukarida tanimladigim configurasyonlara sahip bir 
//eventbus nesnesi uretiyor.Ve eventbusa diyoruz ki  OrderStartedIntegrationEvent i dinlemeye basla eger eventbus uzerinde bir
// OrderStartedIntegrationEvent tetiklenirse bana OrderStartedIntegrationEventHandler kullanarak haber ver.

IEventBus eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

 

app.Run();

