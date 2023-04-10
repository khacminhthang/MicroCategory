using MediatR;

namespace MicroCategory.Domain.Notification
{
    public interface IEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : INotification
    { }
}
