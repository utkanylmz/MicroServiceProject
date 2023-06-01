using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderService.Infrastructure.Context
{
    public class OrderDbContextDesignFactory : IDesignTimeDbContextFactory<OrderDbContext>
    {
        public OrderDbContextDesignFactory()
        {

        }

        public OrderDbContext CreateDbContext(string[] args)
        {
            var connStr = @"Data Source=DESKTOP-O1SR9H9; Initial Catalog=order;Persist Security Info=True;Integrated Security=True;";

            var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>()
                                        .UseSqlServer(connStr);

            return new OrderDbContext(optionsBuilder.Options, new NoMediator());
        }

        class NoMediator : IMediator
        {
            public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
            {
                return Activator.CreateInstance<IAsyncEnumerable<TResponse>>();
            }

            public IAsyncEnumerable<object> CreateStream(object request, CancellationToken cancellationToken = default)
            {
                return Activator.CreateInstance<IAsyncEnumerable<object>>();
            }

            public Task Publish(object notification, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
            {
                return Task.CompletedTask;
            }

            public Task<object> Send(object request, CancellationToken cancellationToken = default)
            {
                return Task.FromResult<object>(default);
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            {
                return Task.FromResult<TResponse>(default);
            }

            

            public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
            {
                throw new NotImplementedException();
            }
        }
    }
}
