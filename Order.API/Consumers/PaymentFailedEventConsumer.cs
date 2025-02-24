using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models.Contexts;
using Shared.Events;

namespace Order.API.Consumers
{
    public class PaymentFailedEventConsumer(OrderAPIDbContext _context) :
        IConsumer<PaymentFailedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var order = await _context.Orders.FindAsync(context.Message.OrderId);
            if (order == null) //nulsa hata fırlat.
                throw new NullReferenceException();

            order.OrderStatus = Enums.OrderStatus.Fail; //ödee gerçekleşmediyse fail yaptık.
            await _context.SaveChangesAsync();
        }
    }
}
