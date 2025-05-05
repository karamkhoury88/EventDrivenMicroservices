var builder = DistributedApplication.CreateBuilder(args);


var rabbitmq = builder.AddRabbitMQ("rabbitmq-messaging")
                      .WithManagementPlugin()
                      .WithDataVolume(isReadOnly: false);

builder.AddProject<Projects.InventoryService>("inventoryservice")
    .WaitFor(rabbitmq)
    .WithReference(rabbitmq);

builder.AddProject<Projects.OrderService>("orderservice")
    .WaitFor(rabbitmq)
    .WithReference(rabbitmq);

builder.Build().Run();
