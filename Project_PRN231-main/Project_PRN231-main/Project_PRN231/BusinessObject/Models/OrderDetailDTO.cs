using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class OrderDetailDTO
    {
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
