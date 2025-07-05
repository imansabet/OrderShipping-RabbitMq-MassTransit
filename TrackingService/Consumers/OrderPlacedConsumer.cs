using MassTransit;
using SharedMessages.Messages;

namespace TrackingSwervice.Consumers;

public class OrderPlacedConsumer : IConsumer<OrderPlaced>
{
    public Task Consume(ConsumeContext<OrderPlaced> context)
    {
        Console.WriteLine($"Order Received For Shipping - {context.Message.OrderId} - quantity {context.Message.Quantity} ");

        return Task.CompletedTask;

    }
}
