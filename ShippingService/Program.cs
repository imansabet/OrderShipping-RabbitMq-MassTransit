using MassTransit;
using ShippingService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit((x) =>
{
    x.AddConsumer<OrderPlacedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");

        cfg.ReceiveEndpoint("shipping-order-queue", e =>
        {
            e.Consumer<OrderPlacedConsumer>(context);

            e.Bind("order-headers-exchange", x =>
            {
                x.ExchangeType = "headers";  
                x.SetBindingArgument("department","shipping");
                x.SetBindingArgument("priority","high");
                x.SetBindingArgument("x-match","all"); // its sure that all headers are match
            });


            //e.Bind("order-placed-exchange", x =>
            //{
            //    x.RoutingKey = "order.shipping";
            //    x.ExchangeType = "direct";
            //});
        });
    });
});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();

