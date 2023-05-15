using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ
{
    public class RabbitMQPersistentConnection : IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly int _retryCount;

        //Hangi connection'ın açık olmadığını kontrol edeceğimiz connection
        private IConnection _connection;
        //Connection aktif mi değil mi? 
        public bool IsConnected => _connection != null && _connection.IsOpen;
        private object lock_object = new object();
        private bool _disposed;
        public RabbitMQPersistentConnection(IConnectionFactory ConnectionFactory,int retryCount=5 )
        {
           _connectionFactory = ConnectionFactory;
            _retryCount = retryCount;
        }
       
        public IModel CreateModel()
        {
            return _connection.CreateModel();
        }
        public void Dispose()
        {
            _disposed = true;
            _connection.Dispose();
        }

        public bool TryConnect()
        {
            lock (lock_object)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_retryCount, retryattempt => TimeSpan.FromSeconds(Math.Pow(2, retryattempt)), (ex, time) =>
                    {
                    }

                );
                policy.Execute(() =>
                {
                    _connection = _connectionFactory.CreateConnection();
                });
                if (IsConnected)
                {
                    _connection.ConnectionShutdown += Connection_ConnectionShutdown;
                    _connection.CallbackException += Connection_CallbackException;
                    _connection.ConnectionBlocked += Connection_ConnectionBlocked;
                    return true;
                }
                return false;
            }
        }

        private void Connection_ConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
        {
            if(_disposed) return;
            TryConnect();
            
        }

        private void Connection_CallbackException(object? sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;
            TryConnect();
        }

        private void Connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            if (_disposed) return;
            TryConnect();
        }
    }
}
