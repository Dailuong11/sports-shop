using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class FilterParams
    {
        public int? categoryId { get; set; }
        public string? productName { get; set; }
        public decimal? UnitPrice { get; set;}

    }
}
