using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    //stokla ilgili işlemleri başarılı bir şekilde tamamladıysak paymentapı ye ödeme yapabilirsin demeliyiz.
    // paymentapı nin ihtiyacı olanları eklemeliyiz.
    public class StockReservedEvent
    {
        public Guid BuyerId { get; set; } //ödeme kimden
        public Guid OrderId{ get; set; } // hangi siparişinden

        public decimal TotalPrice { get; set; }//ne kadar ödeme

        public List<OrderItemMessages> OrderItems { get; set; }
    }
}
