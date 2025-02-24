using MassTransit;
using Shared;
using Shared.Events;
using Stock.API.Consumers;
using Stock.API.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderCreatedEventConsumer>();
    configurator.AddConsumer<PaymentFailedEventConsumer>();

    configurator.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);
    _configure.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e =>
    e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
        _configure.ReceiveEndpoint(RabbitMQSettings.Stock_PaymentFailedEventQueue, e =>
e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
    });
});

builder.Services.AddSingleton<MongoDBService>();

var app = builder.Build();

using IServiceScope scope = app.Services.CreateScope();
MongoDBService mongoDbService =scope.ServiceProvider.GetService<MongoDBService>();

//stok collectiona veri ekleyelim.
var stockCollection = mongoDbService.GetCollection<Stock.API.Models.Stock>();
//program �al��myaa bsa�lay�nca bi kereye mahsus dummy de�i�ken ekleyelim
// i�iinde veri varsa buraya girmeyecek.(de�ilini ald�k.(!))
if (!stockCollection.FindSync(sessionon => true).Any())
{
    await stockCollection.InsertOneAsync(new() { ProductId = Guid.NewGuid().ToString(),Count = 100 });
    await stockCollection.InsertOneAsync(new() { ProductId = Guid.NewGuid().ToString(), Count = 200 });
    await stockCollection.InsertOneAsync(new() { ProductId = Guid.NewGuid().ToString(), Count = 50 });
    await stockCollection.InsertOneAsync(new() { ProductId = Guid.NewGuid().ToString(), Count = 30 });
    await stockCollection.InsertOneAsync(new() { ProductId = Guid.NewGuid().ToString(), Count = 5 });
}

app.Run();
