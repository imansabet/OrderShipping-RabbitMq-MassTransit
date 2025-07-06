using MassTransit;
using SharedMessages.Messages;
using TrackingSwervice.Consumers;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit((x) =>
{ 
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");


        cfg.Message<OrderPlaced>(x => x.SetEntityName("order-headers-exchange"));

        //cfg.Publish<OrderPlaced>(x => x.ExchangeType = "direct");
        //cfg.Publish<OrderPlaced>(x => x.ExchangeType = "fanout");
        cfg.Publish<OrderPlaced>(x => x.ExchangeType = "headers");

    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapPost("/orders" , async (OrderRequest order , IBus bus) =>
{
    var orderPlacedMessage = new OrderPlaced(order.orderId, order.quantity);

    var headers = new Dictionary<string, object>();

    if(order.quantity > 10)
    {
        headers["department"] = "shipping";
        headers["priority"] = "high";
    }
    else
    {
        headers["department"] = "tracking";
        headers["priority"] = "low";
    }
    await bus.Publish(orderPlacedMessage , context =>
    {
        context.Headers.Set("department", headers["department"]);
        context.Headers.Set("priority", headers["priority"]);
    });
        //string routingKey = order.quantity > 10 ? "order.shipping" : "order.regular.tracking";

    //await bus.Publish(orderPlacedMessage , async context =>
    //{
    //    context.SetRoutingKey(routingKey);
    //});

        return Results.Created($"/orders/{order.orderId}", orderPlacedMessage); 
});



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

public record OrderRequest(Guid orderId , int quantity);