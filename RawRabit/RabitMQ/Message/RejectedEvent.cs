
using System.Text.Json.Serialization;

namespace MicroCategory.Infrastructure.RabitMQ.Message
{
    /// <summary>
    /// Implement RejectedEvent
    /// </summary>
    public class RejectedEvent : IRejectedEvent
    {
        public string Reason { get; }
        public string Code { get; }

        /// <summary>
        /// Initialize a new instance of the <see cref="RejectedEvent"/> class
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="code"></param>
        [JsonConstructor]
        public RejectedEvent(string reason, string code)
        {
            Reason = reason;
            Code = code;
        }

        /// <summary>
        /// IRejectedEvent
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IRejectedEvent For(string name)
        {
            return new RejectedEvent($"Xảy ra lỗi khi thực hiện: " +
                                    $"{name}", $"{name}_error");
        }
    }
}
