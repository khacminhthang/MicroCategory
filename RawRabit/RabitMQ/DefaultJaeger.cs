
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using OpenTracing;
using System.Reflection;

namespace MicroCategory.Infrastructure.RabitMQ
{
    /// <summary>
    /// DefaultJaeger
    /// </summary>
    public class DefaultJaeger
    {
        public static ITracer Create()
        {
            return new Tracer.Builder(Assembly.GetEntryAssembly().FullName)
          .WithReporter(new NoopReporter())
          .WithSampler(new ConstSampler(false))
          .Build();
        }
    }
}
