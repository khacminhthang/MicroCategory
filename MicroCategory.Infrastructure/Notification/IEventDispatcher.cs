using MediatR;

namespace MicroCategory.Infrastructure.Notification
{
    public interface IEventDispatcher
    {
        /// <summary>
        /// RaiseEvent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <returns></returns>
        Task RaiseEvent<T>(T @event) where T : INotification;
    }
}
