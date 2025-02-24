using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models.Contexts;
using Shared.Events;

namespace Order.API.Consumers
{
    public class StockNotReservedEventConsumer(OrderAPIDbContext _context) : IConsumer<StockNotReservedEvent>
    {
        //stokapi de bir sorun olduğunda orderapi de oluşturulan siparişi faile çekelim.
        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            var order = await _context.Orders.FindAsync(context.Message.OrderId);
            if (order == null) //nulsa hata fırlat.
                throw new NullReferenceException();

            order.OrderStatus = Enums.OrderStatus.Fail; //ödee gerçekleşmediyse fail yaptık.
            await _context.SaveChangesAsync();
        }
    }
}
