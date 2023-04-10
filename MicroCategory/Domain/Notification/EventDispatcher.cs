using MediatR;

namespace MicroCategory.Domain.Notification
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDispatcher"/> class
        /// </summary>
        /// <param name="mediator"></param>
        public EventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// RaiseEvent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task RaiseEvent<T>(T @event) where T : INotification
        {
            return _mediator.Publish(@event);
        }

    }
}
