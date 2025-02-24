using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class PaymentCompletedEvent
    {
        //siparişin başarılı ödeme yapıldığını belirteceğiz.ödeme başarılı.
        //başarılı ise ıd bilinse yeterli.
        public Guid OrderId { get; set; }
    }
}
