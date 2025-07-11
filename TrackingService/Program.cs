using MassTransit;
using TrackingSwervice.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit((x) =>
{
    x.AddConsumer<OrderPlacedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");

        cfg.ReceiveEndpoint("tracking-order-placed", e =>
        {
            e.Consumer<OrderPlacedConsumer>(context);

            e.Bind("order-headers-exchange", x =>
            {
                x.ExchangeType = "headers";
                x.SetBindingArgument("department", "tracking");
                x.SetBindingArgument("priority", "low");
                x.SetBindingArgument("x-match", "all"); // its sure that all headers are match
            });

            //e.Bind("order-topic-exchange", x =>
            //{            
            //    x.RoutingKey = "order.#";
            //    x.ExchangeType = "topic";
            //});

            //e.Bind("order-placed-exchange", x =>
            //{
            //    x.RoutingKey = "order.tracking";
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
