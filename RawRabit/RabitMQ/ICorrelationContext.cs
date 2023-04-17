namespace MicroCategory.Infrastructure.RabitMQ
{
    /// <summary>
    /// Interface ICorrelationContext
    /// </summary>
    public interface ICorrelationContext
    {
        Guid Id { get; }
        string SpanContext { get; }
        string ConnectionId { get; }
        int Retries { get; set; }
    }
}