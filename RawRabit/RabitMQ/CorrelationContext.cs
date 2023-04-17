
using Newtonsoft.Json;

namespace MicroCategory.Infrastructure.RabitMQ
{
    /// <summary>
    /// Implement CorrelationContext 
    /// </summary>
    public class CorrelationContext : ICorrelationContext
    {
        public Guid Id { get; }
        public string SpanContext { get; }
        public string ConnectionId { get; }
        public int Retries { get; set; }

        public CorrelationContext() { }

        /// <summary>
        /// Empty
        /// </summary>
        public static ICorrelationContext Empty => new CorrelationContext();

        private CorrelationContext(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// CorrelationContext Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ICorrelationContext FromId(Guid id)
            => new CorrelationContext(id);

        /// <summary>
        /// CorrelationContext
        /// </summary>
        /// <param name="id"></param>
        /// <param name="traceId"></param>
        /// <param name="spanContext"></param>
        /// <param name="connectionId"></param>
        /// <param name="name"></param>
        /// <param name="origin"></param>
        /// <param name="retries"></param>
        [JsonConstructor]
        private CorrelationContext(Guid id, string spanContext, string connectionId, int retries)
        {
            Id = id;
            SpanContext = spanContext;
            ConnectionId = connectionId;
            Retries = retries;
        }

        /// <summary>
        /// ICorrelationContext
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ICorrelationContext From<T>(ICorrelationContext context)
        {
            return Create<T>(context.Id, context.SpanContext, context.ConnectionId);
        }

        /// <summary>
        /// ICorrelationContext
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="traceId"></param>
        /// <param name="spanContext"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public static ICorrelationContext Create<T>(Guid id, string spanContext, string connectionId)
        {
            return new CorrelationContext(id, spanContext, connectionId, 0);
        }
    }
}
