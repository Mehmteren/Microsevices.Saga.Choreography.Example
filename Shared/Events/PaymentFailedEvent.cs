using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class PaymentFailedEvent
    {
        public Guid OrderId { get; set; }
        public string Message { get; set; }//neden iptal olduğu.

        //iptal olduğunda bunu orderapi ye bildirmeliyiz ki sipariş iptal edilsin
       
        // stockapi ye bilidrilsin ki stok durumu eski hale gelsin .
        public List<OrderItemMessages> OrderItems { get; set; }

    }
}
