using MediatR;
using MicroCategory.Domain.Notification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MicroCategory.Controllers
{
    [Produces("application/json")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Fields
        /// </summary>
        private readonly IMediator _mediator;
        private readonly DomainNotificationHandler _notificationHandler;

        /// <summary>
        /// Initialize a new instance of the <see cref="BaseController"/> class
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="notificationHandler"></param>
        public BaseController(IMediator mediator,
                              INotificationHandler<DomainNotification> notificationHandler)
        {
            _mediator = mediator ?? throw new ArgumentNullException();
            _notificationHandler = (DomainNotificationHandler)notificationHandler;
        }

        /// <summary>
        /// Query async
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        protected async Task<TResult> QueryAsync<TResult>(IRequest<TResult> query)
        {
            return await _mediator.Send(query);
        }

        /// <summary>
        /// Command async
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        protected async Task<TResult> CommandAsync<TResult>(IRequest<TResult> command)
        {
            return await _mediator.Send(command);
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>The errors.</value>
        protected IEnumerable<string> Errors => _notificationHandler.Notifications.Select(n => n.Value);
    }
}
