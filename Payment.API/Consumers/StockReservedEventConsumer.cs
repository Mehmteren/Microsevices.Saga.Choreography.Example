using MassTransit;
using Shared.Events;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer(IPublishEndpoint publishEndpoint) : IConsumer<StockReservedEvent>
    {
        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
         //ödeme ile ilgili işlemler
         if (true)
            {
                //ödeme başarılı...paymentcompletedevent fırlatılıcak. 
                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                };
                await publishEndpoint.Publish(paymentCompletedEvent);
                await Console.Out.WriteLineAsync("Ödeme başarılı...");
            }
         else
            {
                //ödeme başarısız...paymentfailedevetn fırlatılıcak.
                PaymentFailedEvent paymentFailedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    Message = "Yetersiz bakiye ",
                    OrderItems = context.Message.OrderItems,
                };
                await publishEndpoint.Publish(paymentFailedEvent);
                await Console.Out.WriteLineAsync("Ödeme başarısız...");

            }
        }
    }
}

