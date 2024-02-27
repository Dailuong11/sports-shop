using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class OrderDto
    {
        public string name { get; set; }
        public string action { get; set; }
        public Customer Customer { get; set; }
    }
}
