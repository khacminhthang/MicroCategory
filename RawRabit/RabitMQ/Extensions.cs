using Autofac;
using MicroCategory.Infrastructure.RabitMQ.Message;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using OpenTracing;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Configuration;
using RawRabbit.Enrichers.MessageContext;
using RawRabbit.Instantiation;
using RawRabbit.Pipe;
using RawRabbit.Pipe.Middleware;
using System.Reflection;

namespace MicroCategory.Infrastructure.RabitMQ
{
    /// <summary>
    /// Extensions Class
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// UseRabbitMq
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IBusSubscriber UseRabbitMq(this IApplicationBuilder app)
        {
            return new BusSubscriber(app);
        }

        /// <summary>
        /// AddRabbitMq
        /// </summary>
        /// <param name="containerBuilder"></param>
        public static void AddRabbitMq(this ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var options = configuration.GetOptions<RabbitMqOptions>("rabbitMq");

                return options;
            }).SingleInstance();

            builder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var options = configuration.GetOptions<RawRabbitConfiguration>("rabbitMq");

                return options;
            }).SingleInstance();

            var assembly = Assembly.GetCallingAssembly();

            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(IEventHandlerRabbitMq<>))
                .InstancePerDependency();

            builder.RegisterType<BusPublisher>().As<IBusPublisher>()
                                    .InstancePerDependency();
            builder.RegisterInstance(DefaultJaeger.Create())
                   .As<ITracer>()
                   .SingleInstance()
                   .PreserveExistingDefaults();

            ConfigureBus(builder);
        }

        /// <summary>
        /// ConfigureBus
        /// </summary>
        /// <param name="builder"></param>
        private static void ConfigureBus(ContainerBuilder builder)
        {
            builder.Register<IInstanceFactory>(context =>
            {
                var options = context.Resolve<RabbitMqOptions>();
                var configuration = context.Resolve<RawRabbitConfiguration>();
                var namingConventions = new CustomNamingConventions(options.Namespace);
                var tracer = context.Resolve<ITracer>();

                return RawRabbitFactory.CreateInstanceFactory(new RawRabbitOptions
                {
                    DependencyInjection = ioc =>
                    {
                        ioc.AddSingleton(options);
                        ioc.AddSingleton(configuration);
                        ioc.AddSingleton<INamingConventions>(namingConventions);
                        ioc.AddSingleton(tracer);
                    },
                    Plugins = p => p
                        .UseAttributeRouting()
                        .UseRetryLater()
                        .UpdateRetryInfo()
                        .UseMessageContext<CorrelationContext>()
                        .UseContextForwarding()
                });
            }).SingleInstance();

            builder.Register(context => context.Resolve<IInstanceFactory>().Create());
        }

        /// <summary>
        /// CustomNamingConventions
        /// </summary>
        private class CustomNamingConventions : NamingConventions
        {
            public CustomNamingConventions(string defaultNamespace)
            {
                var assemblyName = Assembly.GetEntryAssembly().GetName().Name;
                ExchangeNamingConvention = type => GetNamespace(type, defaultNamespace).ToLowerInvariant();
                RoutingKeyConvention = type =>
                    $"{GetRoutingKeyNamespace(type, defaultNamespace)}{type.Name.Underscore()}".ToLowerInvariant();
                QueueNamingConvention = type => GetQueueName(assemblyName, type, defaultNamespace);
                ErrorExchangeNamingConvention = () => $"{defaultNamespace}.error";
                RetryLaterExchangeConvention = span => $"{defaultNamespace}.retry";
                RetryLaterQueueNameConvetion = (exchange, span) =>
                    $"{defaultNamespace}.retry_for_{exchange.Replace(".", "_")}_in_{span.TotalMilliseconds}_ms".ToLowerInvariant();
            }

            /// <summary>
            /// GetRoutingKeyNamespace
            /// </summary>
            /// <param name="type"></param>
            /// <param name="defaultNamespace"></param>
            /// <returns></returns>
            private static string GetRoutingKeyNamespace(Type type, string defaultNamespace)
            {
                var @namespace = type.GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? defaultNamespace;

                return string.IsNullOrWhiteSpace(@namespace) ? string.Empty : $"{@namespace}.";
            }

            /// <summary>
            /// GetNamespace
            /// </summary>
            /// <param name="type"></param>
            /// <param name="defaultNamespace"></param>
            /// <returns></returns>
            private static string GetNamespace(Type type, string defaultNamespace)
            {
                var @namespace = type.GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? defaultNamespace;

                return string.IsNullOrWhiteSpace(@namespace) ? type.Name.Underscore() : $"{@namespace}";
            }

            /// <summary>
            /// GetQueueName
            /// </summary>
            /// <param name="assemblyName"></param>
            /// <param name="type"></param>
            /// <param name="defaultNamespace"></param>
            /// <returns></returns>
            private static string GetQueueName(string assemblyName, Type type, string defaultNamespace)
            {
                var @namespace = type.GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? defaultNamespace;
                var separatedNamespace = string.IsNullOrWhiteSpace(@namespace) ? string.Empty : $"{@namespace}.";

                return $"{assemblyName}/{separatedNamespace}{type.Name.Underscore()}".ToLowerInvariant();
            }
        }

        /// <summary>
        /// RetryStagedMiddleware
        /// </summary>
        private class RetryStagedMiddleware : StagedMiddleware
        {
            public override string StageMarker { get; } = global::RawRabbit.Pipe.StageMarker.MessageDeserialized;

            public override async Task InvokeAsync(IPipeContext context,
                CancellationToken token = new CancellationToken())
            {
                var retry = context.GetRetryInformation();
                if (context.GetMessageContext() is CorrelationContext message)
                {
                    message.Retries = retry.NumberOfRetries;
                }

                await Next.InvokeAsync(context, token);
            }
        }

        /// <summary>
        /// IClientBuilder
        /// </summary>
        /// <param name="clientBuilder"></param>
        /// <returns></returns>
        private static IClientBuilder UpdateRetryInfo(this IClientBuilder clientBuilder)
        {
            clientBuilder.Register(c => c.Use<RetryStagedMiddleware>());

            return clientBuilder;
        }
    }
}
