using MediatR;
using MicroCategory.Infrastructure.RabitMQ.Message;
using MicroCategory.Infrastructure.RabitMQ.Types;

namespace MicroCategory.Infrastructure.RabitMQ
{
    /// <summary>
    /// Interface IBusSubscriber
    /// </summary>
    public interface IBusSubscriber
    {
        /// <summary>
        /// SubscriberEvent
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="namespace"></param>
        /// <param name="queueName"></param>
        /// <param name="funcError"></param>
        /// <returns></returns>
        IBusSubscriber SubscriberEvent<TEvent>(string @namespace = null,
                                                string queueName = null,
                                                Func<TEvent, CommonException, IRejectedEvent> funcError = null) where TEvent : INotification;
    }
}
