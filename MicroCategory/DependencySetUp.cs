using MediatR;
using MicroCategory.Application.MappingConfigurations;
using MicroCategory.Domain.Models;
using MicroCategory.Domain.Repositories.Implement;
using MicroCategory.Domain.Repositories.Interface;
using MicroCategory.Domain.UnitOfWork;
using MicroCategory.Infrastructure.Notification;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using System.Reflection;

namespace MicroCategory
{
    public static class DependencySetUp
    {
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            // Add MediatR
            services.AddMediatR(Assembly.GetExecutingAssembly());

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork<DatabaseContext>>();

            // Repository
            services.AddScoped<ICTermRepository, CTermRepository>();
            services.AddScoped<ICTermmetumRepository, CTermmetumRepository>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
            services.AddControllers();

            // Auto Mapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            // Write logs to a file for warning and logs with a higher severity
            // Logs are written in JSON
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(new JsonFormatter(),
                    "Logs/important-logs.json",
                    restrictedToMinimumLevel: LogEventLevel.Warning)
                // Add a log file that will be replaced by a new log file each day
                .WriteTo.File("Logs/all-daily-.logs",
                    rollingInterval: RollingInterval.Day)
                // Set default minimum log level
                .MinimumLevel.Debug()
                // Create the actual logger
                .CreateLogger();
            Log.CloseAndFlush();

            return services;
        }
    }
}
