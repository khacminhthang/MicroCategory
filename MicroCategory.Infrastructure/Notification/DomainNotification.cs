using MediatR;

namespace MicroCategory.Infrastructure.Notification
{
    public class DomainNotification : INotification
    {
        /// <summary>
        /// DomainNotificationId
        /// </summary>
        public Guid DomainNotificationId { get; private set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; private set; }

        /// <summary>
        /// Version
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainNotification"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public DomainNotification(string key, string value)
        {
            DomainNotificationId = Guid.NewGuid();
            Key = key;
            Value = value;
            Version = 1;
        }
    }
}
