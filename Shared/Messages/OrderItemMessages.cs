using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class OrderItemMessages
    {
        public Guid ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
