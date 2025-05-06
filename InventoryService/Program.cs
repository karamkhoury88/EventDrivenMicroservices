using Common.Services;
using InventoryService.Data;
using InventoryService.RabbitMq;
using RabbitMQ.Client;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.AddServiceDefaults();

// Add services to the container.
builder.Services.RegisterMessageQueueService();
builder.Services.AddHostedService<RabbitMqConsumerService>();


// Register the RabbitMQ client
builder.AddRabbitMQClient("rabbitmq-messaging", configureConnectionFactory: factory =>
{
    // Configure the RabbitMQ connection factory
    if (factory is IAsyncConnectionFactory asyncFactory)
    {
        // Enable async dispatching of consumers
        asyncFactory.DispatchConsumersAsync = true;
    }
});

// Get the connection string for the database
string dbConnectionString = builder.Configuration.GetConnectionString("InventoryDb");

// Register the DbContext with SQLite provider
builder.Services.AddSqlite<InventoryDbContext>(dbConnectionString);

// Add Controllers
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(_ => _.Servers = []);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Apply any pending migrations and create the database if it doesn't exist
await app.MigrateDbAsync();

app.Run();