using MediatR;
using MicroCategory.Application.MappingConfigurations;
using MicroCategory.Domain.Models;
using MicroCategory.Domain.Notification;
using MicroCategory.Domain.Repositories.Implement;
using MicroCategory.Domain.Repositories.Interface;
using MicroCategory.Domain.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add MediatR
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork<DatabaseContext>>();

// Repository
builder.Services.AddScoped<ICTermRepository, CTermRepository>();
builder.Services.AddScoped<ICTermmetumRepository, CTermmetumRepository>();
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
builder.Services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

builder.Services.AddControllers();

// Auto Mapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
