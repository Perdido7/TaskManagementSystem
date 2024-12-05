using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TaskManagementSystem;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.Business.RabbitMQ;
using TaskManagementSystem.Business.Sent;
using TaskManagementSystem.DataAccess;
using TaskManagementSystem.Options;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TaskManagementSystemDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("TaskManagementSystemDbConnection")));
builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ISentService, SentService>();

builder.Services.AddSingleton<ServiceBusHandler>();
builder.Services.AddScoped<TaskCreateProcessor>();
builder.Services.AddScoped<TaskUpdateProcessor>();
builder.Services.AddHostedService<TaskCreateBackgroundService>();
builder.Services.AddHostedService<TaskUpdateBackgroundService>();

builder.Services.AddAutoMapper(typeof(TaskMappingProfile));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
