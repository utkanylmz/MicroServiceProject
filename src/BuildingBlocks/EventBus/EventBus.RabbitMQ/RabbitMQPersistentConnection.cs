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
        private readonly int retryCount;
        private IConnection _connection;
        private object lock_object = new object();
        public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, int retryCount = 5)
        {
            _connectionFactory = connectionFactory;
            this.retryCount = retryCount;
        }

        private bool IsDisposed;
        public bool IsConnected => _connection != null && _connection.IsOpen;
        public bool IsConnection => _connection != null && _connection.IsOpen;

        public IModel CreateModel()
        {
            return _connection.CreateModel();
        }

        public void Dispose()
        {
            IsDisposed = true;
            _connection.Dispose();
        }

        public bool TryConnect()
        {
            lock (lock_object)
            {
                var policy = Policy.Handle<SocketException>().Or<BrokerUnreachableException>()
                    .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {

                    });
                policy.Execute(() =>
                {
                    _connection = _connectionFactory.CreateConnection();
                });
                if (IsConnected)
                {
                    _connection.ConnectionShutdown += _connection_ConnectionShutdown;
                    _connection.CallbackException += _connection_CallbackException;
                    _connection.ConnectionBlocked += _connection_ConnectionBlocked;
                    //log

                    return true;
                }
                return false;
            }
        }

        private void _connection_ConnectionBlocked(object? sender, global::RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
            if (!IsDisposed)
                TryConnect();
        }

        private void _connection_CallbackException(object? sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
        {
            if (!IsDisposed)
                TryConnect();
        }

        private void _connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            //log Connection_ConnectionsShutdown
            if (!IsDisposed)
                TryConnect();
        }
    }
}
