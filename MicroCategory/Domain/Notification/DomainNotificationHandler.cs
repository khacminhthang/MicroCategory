using System.Collections.ObjectModel;

namespace MicroCategory.Domain.Notification
{
    public class DomainNotificationHandler : IEventHandler<DomainNotification>
    {
        /// <summary>
        /// The notifications
        /// </summary>
        private List<DomainNotification> _notifications;

        /// <summary>
        /// Gets the notifications.
        /// </summary>
        /// <value>The notifications.</value>
        public ReadOnlyCollection<DomainNotification> Notifications => _notifications.AsReadOnly();

        /// <summary>
        /// Initialize a new instance of the <see cref="DomainNotificationHandler"/> class
        /// </summary>
        public DomainNotificationHandler()
        {
            _notifications = new List<DomainNotification>();
        }

        /// <summary>
        /// Handles the specified notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task Handle(DomainNotification notification, CancellationToken cancellationToken)
        {
            _notifications.Add(notification);
            return Task.CompletedTask;
        }
    }
}
