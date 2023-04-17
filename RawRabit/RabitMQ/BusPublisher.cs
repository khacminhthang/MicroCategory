using MediatR;
using RawRabbit;
using RawRabbit.Enrichers.MessageContext;

namespace MicroCategory.Infrastructure.RabitMQ
{
    /// <summary>
    /// Implement BusPublisher
    /// </summary>
    public class BusPublisher : IBusPublisher
    {
        private readonly IBusClient _busClient;

        /// <summary>
        /// Initialize a new instance of the <see cref="BusPublisher"/> class
        /// </summary>
        /// <param name="busClient"></param>
        public BusPublisher(IBusClient busClient)
        {
            _busClient = busClient ?? throw new ArgumentNullException(nameof(busClient));
        }

        /// <summary>
        /// PublishAsync
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task PublishAsync<TEvent>(TEvent @event, ICorrelationContext context) where TEvent : INotification
        {
            await _busClient.PublishAsync(@event, ctx => ctx.UseMessageContext(context));
        }
    }
}
