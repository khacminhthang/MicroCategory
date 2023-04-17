
using MediatR;
using MicroCategory.Infrastructure.RabitMQ.Message;
using MicroCategory.Infrastructure.RabitMQ.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Tag;
using Polly;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Enrichers.MessageContext;

namespace MicroCategory.Infrastructure.RabitMQ
{
    /// <summary>
    /// Implement BusSubscriber
    /// </summary>
    public class BusSubscriber : IBusSubscriber
    {
        private readonly ILogger _logger;
        private readonly IBusClient _busClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITracer _tracer;
        private readonly int _retries;
        private readonly int _retryInterval;


        /// <summary>
        /// Initialize a new instance of the <see cref="BusSubscriber"/> class
        /// </summary>
        /// <param name="app"></param>
        public BusSubscriber(IApplicationBuilder app)
        {
            _logger = app.ApplicationServices.GetService<ILogger<BusSubscriber>>();
            _serviceProvider = app.ApplicationServices.GetService<IServiceProvider>();
            _busClient = app.ApplicationServices.GetService<IBusClient>();
            _tracer = app.ApplicationServices.GetService<ITracer>();
            var options = app.ApplicationServices.GetService<RabbitMqOptions>();
            _retries = options.Retries >= 0 ? options.Retries : 3;
            _retryInterval = options.RetryInterval > 0 ? options.RetryInterval : 2;

        }

        /// <summary>
        /// SubscriberEvent
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="namespace"></param>
        /// <param name="queueName"></param>
        /// <param name="funcError"></param>
        /// <returns></returns>
        public IBusSubscriber SubscriberEvent<TEvent>(string @namespace = null,
                                                    string queueName = null,
                                                    Func<TEvent, CommonException, IRejectedEvent> funcError = null) where TEvent : INotification
        {
            _busClient.SubscribeAsync<TEvent, CorrelationContext>(async (@event, correlationContext) =>
            {
                var eventHandler = _serviceProvider.GetService<IEventHandlerRabbitMq<TEvent>>();

                return await TryHandleAsync(@event, correlationContext, () => eventHandler.HandleAsync(@event, correlationContext), funcError);
            });

            return this;
        }


        /// <summary>
        /// TryHandleAsync
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <param name="correlationContext"></param>
        /// <param name="handle"></param>
        /// <param name="funcError"></param>
        /// <returns></returns>
        private async Task<Acknowledgement> TryHandleAsync<TMessage>(TMessage message,
                                                                    CorrelationContext correlationContext,
                                                                    Func<Task> handle,
                                                                    Func<TMessage, CommonException, IRejectedEvent> funcError = null)
        {
            var currentRetry = 0;
            var retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(_retries, i => TimeSpan.FromSeconds(_retryInterval));

            var messageName = message.GetType().Name;

            return await retryPolicy.ExecuteAsync<Acknowledgement>(async () =>
            {

                var scope = _tracer
                        .BuildSpan("executing-handler")
                        .AsChildOf(_tracer.ActiveSpan)
                        .StartActive(true);

                using (scope)
                {
                    var span = scope.Span;

                    try
                    {
                        var retryMessage = currentRetry == 0 ? string.Empty : $"Retry: {currentRetry}'.";

                        var preLogMessage = $"Handling a message: '{messageName}' " +
                                  $"with correlation id: '{correlationContext.Id}'. {retryMessage}";

                        _logger.LogInformation(preLogMessage);
                        span.Log(preLogMessage);

                        await handle();

                        var postLogMessage = $"Handled a message: '{messageName}' " +
                                             $"with correlation id: '{correlationContext.Id}'. {retryMessage}";
                        _logger.LogInformation(postLogMessage);
                        span.Log(postLogMessage);

                        return new Ack();
                    }
                    catch (Exception exception)
                    {
                        currentRetry++;
                        _logger.LogError(exception, exception.Message);
                        span.Log(exception.Message);
                        span.SetTag(Tags.Error, true);

                        if (exception is CommonException commonException && funcError != null)
                        {
                            var rejectedEvent = funcError(message, commonException);
                            await _busClient.PublishAsync(rejectedEvent, ctx => ctx.UseMessageContext(correlationContext));
                            _logger.LogInformation($"Published a rejected event: '{rejectedEvent.GetType().Name}' " +
                                                   $"for the message: '{messageName}' with correlation id: '{correlationContext.Id}'.");

                            span.SetTag("error-type", "domain");
                            return new Ack();
                        }

                        span.SetTag("error-type", "infrastructure");
                        throw new Exception($"Unable to handle a message: '{messageName}' " +
                                            $"with correlation id: '{correlationContext.Id}', " +
                                            $"retry {currentRetry - 1}/{_retries}...");
                    }
                }
            });
        }
    }
}
