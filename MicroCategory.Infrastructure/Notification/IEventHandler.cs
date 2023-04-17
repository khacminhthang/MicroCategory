using MediatR;

namespace MicroCategory.Infrastructure.Notification
{
    public interface IEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : INotification
    { }
}
