using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Res
{
    public class OrderDetailRes
    {
        public int OrderId { get; set; }
        public string? ProductName { get; set; }
        public string? Category { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
    }
}
