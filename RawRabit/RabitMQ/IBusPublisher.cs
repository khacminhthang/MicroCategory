using MediatR;

namespace MicroCategory.Infrastructure.RabitMQ
{
    /// <summary>
    /// Interface IBusPublisher
    /// </summary>
    public interface IBusPublisher
    {
        /// <summary>
        /// PulishAsync
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event"></param>
        /// <returns></returns>
        Task PublishAsync<TEvent>(TEvent @event, ICorrelationContext context) where TEvent : INotification;
    }
}
