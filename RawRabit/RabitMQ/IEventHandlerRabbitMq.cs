
using MediatR;

namespace MicroCategory.Infrastructure.RabitMQ
{
    /// <summary>
    /// IEventHandlerRabbitMq
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IEventHandlerRabbitMq<in TEvent> where TEvent : INotification
    {
        Task HandleAsync(TEvent @event, ICorrelationContext context);
    }
}
