using MediatR;

namespace MicroCategory.Infrastructure.RabitMQ.Message
{
    public interface IRejectedEvent : INotification
    {
        string Reason { get; }
        string Code { get; }
    }
}
