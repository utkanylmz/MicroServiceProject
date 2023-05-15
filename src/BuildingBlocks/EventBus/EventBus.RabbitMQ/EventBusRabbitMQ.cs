﻿using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        private readonly IConnectionFactory connectionFactory;
        RabbitMQPersistentConnection persistentConnection;
        private readonly IModel consumerChannel;
        public EventBusRabbitMQ(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            if (config.Connection != null) 
            {
                var connJson = JsonConvert.SerializeObject(EventBusConfig, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connJson);
            }
            else
            {
                connectionFactory = new ConnectionFactory();
            }
            persistentConnection = new RabbitMQPersistentConnection(connectionFactory,config.ConnectionRetryCount);
            consumerChannel = CreateConsumerChannel();
            SubsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        
        public override void Publish(IntegrationEvent @event)
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
               .Or<SocketException>()
               .WaitAndRetry(EventBusConfig.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
               {
                   // loglama işlemleri 
               });

            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            consumerChannel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct");  // Ensure exchange exists while publishing

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);


            policy.Execute(() =>
            {
                var properties = consumerChannel.CreateBasicProperties();
                properties.DeliveryMode = 2;  // persistent
                //consumerChannel.QueueDeclare(
             
                //    queue: GetSubName(eventName),  //Queuenun olup olmadığının kontrolü
                //    durable: true,
                //    exclusive: false,
                //    autoDelete: false,
                //    arguments: null);
                //_consumerChannel.QueueBind(queue: GetSubName(eventName),
                //    exchange: EventBusConfig.DefaultTopicName,
                //    routingKey: eventName);
               
                consumerChannel.BasicPublish( //Publish metodu ile mesajı gönderme
                    exchange: EventBusConfig.DefaultTopicName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });

        }

        public override void Subscribe<T, TH>()
        {
            var eventName=typeof(T).Name;
            eventName=ProcessEventName(eventName);
            //Dışarıdan Eventin adi gönderildiğinde zaten bu event'i subscribe olup dinleyip dinlemediğimiz kontrol ediyoruz.
            if (!SubsManager.HasSubscriptionForEvent(eventName))
            {
                if (!persistentConnection.IsConnected)
                {
                    persistentConnection.TryConnect();

                }
                consumerChannel.QueueDeclare( //Queue oluşturuyoruz
                    queue: GetSubName(eventName),
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                consumerChannel.QueueBind(//QueueYu ilgili channel'a bind ediyoruz.
                     queue: GetSubName(eventName),
                     exchange: EventBusConfig.DefaultTopicName,
                     routingKey: eventName);
            }

            SubsManager.AddSubscription<T, TH>();
            StartBasicConsume(eventName);
        }

        public override void UnSubscribe<T, TH>()
        {
            SubsManager.RemoveSubscription<T, TH>(); 
        }


        //Channel oluşturan metot
        private IModel CreateConsumerChannel()
        {
            // Bir channela bağlı mısın? 
            if (!persistentConnection.IsConnected)
            {
                //Bağlanmayı dene
                persistentConnection.TryConnect();

            }
            //Channel oluştur ismi configden gelen isim exchange type'ı direct geriye channel'ı dön.
            var channel = persistentConnection.CreateModel();
            channel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName,
                                    type: "direct");
            return channel;

        }

        private void StartBasicConsume(string eventName)
        {
            if (consumerChannel != null)
            {
                var consumer = new /*Async*/EventingBasicConsumer(consumerChannel);
                consumer.Received += Consumer_Received;
                consumerChannel.BasicConsume(
                    queue: GetSubName(eventName),
                    autoAck: false,
                    consumer: consumer);
            }
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            eventName = ProcessEventName(eventName);
            var message = Encoding.UTF8.GetString(e.Body.Span);
            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception)
            {
                //logging mekanizması kullanabilir.
            }
            consumerChannel.BasicAck(e.DeliveryTag, multiple: false);
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            eventName = ProcessEventName(eventName);

            if (!persistentConnection.IsConnected)
                persistentConnection.TryConnect();

            consumerChannel.QueueUnbind(
                queue: eventName,
                exchange: EventBusConfig.DefaultTopicName,
                routingKey: eventName);

            if (SubsManager.IsEmpty)
            {
                consumerChannel.Close();
            }
        }
    }
}
